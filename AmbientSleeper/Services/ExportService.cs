using System.Text.Json;
using AmbientSleeper.Models;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel.Communication;
using AmbientSleeper.Resources.Strings;
using Microsoft.Extensions.Logging;

namespace AmbientSleeper.Services;

public class ExportService : IExportService
{
    private readonly FeatureGate _features;
    private readonly ILogger<ExportService>? _logger;

    public ExportService(FeatureGate features, ILogger<ExportService>? logger = null)
    {
        _features = features ?? throw new ArgumentNullException(nameof(features));
        _logger = logger;
    }

    public async Task<string> ExportAsync(ExportScope scope, bool shareViaEmail = false, CancellationToken ct = default)
    {
        if (!_features.ExportEnabled)
        {
            var error = "Export is available on Standard, Premium, and Pro+ tiers.";
            _logger?.LogWarning(error);
            throw new InvalidOperationException(error);
        }

        string? path = null;
        try
        {
            _logger?.LogInformation($"Starting export with scope: {scope}, shareViaEmail: {shareViaEmail}");

            var pkg = new MixExportPackage
            {
                Scope = scope,
                TierRequirement = scope == ExportScope.Shareable ? "Premium/Pro+" : "Standard+",
                Mixes = UserPreferences.GetMixes(),
                Playlists = UserPreferences.GetPlaylists()
            };

            _logger?.LogInformation($"Export package contains {pkg.Mixes?.Count ?? 0} mixes and {pkg.Playlists?.Count ?? 0} playlists");

            var json = JsonSerializer.Serialize(pkg, new JsonSerializerOptions { WriteIndented = true });
            var fileName = $"AmbientSleeper_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}_{scope}.json";
            path = Path.Combine(FileSystem.CacheDirectory, fileName);
            
            await File.WriteAllTextAsync(path, json, ct).ConfigureAwait(false);
            _logger?.LogInformation($"Export file written to: {path}");

            if (shareViaEmail)
            {
                try
                {
                    var subject = AppResources.ResourceManager.GetString("ExportEmail_Subject") ?? "AmbientSleeper Export";
                    var message = new EmailMessage { Subject = subject };
                    
                    if (message.Attachments == null)
                    {
                        _logger?.LogWarning("EmailMessage.Attachments is null, falling back to share sheet");
                        throw new InvalidOperationException("Email attachments not available");
                    }

                    message.Attachments.Add(new EmailAttachment(path));
                    await Email.ComposeAsync(message).ConfigureAwait(false);
                    _logger?.LogInformation("Email composer opened successfully");
                }
                catch (Exception emailEx)
                {
                    _logger?.LogWarning(emailEx, "Email sharing failed, falling back to share sheet");
                    // If email not available, fall back to share sheet
                    try
                    {
                        await Share.Default.RequestAsync(new ShareFileRequest
                        {
                            Title = AppResources.ExportComplete_Title,
                            File = new ShareFile(path)
                        }).ConfigureAwait(false);
                        _logger?.LogInformation("Share sheet fallback completed");
                    }
                    catch (Exception shareEx)
                    {
                        _logger?.LogError(shareEx, "Share sheet fallback also failed");
                        throw new InvalidOperationException("Failed to share export file via email or share sheet.", shareEx);
                    }
                }
            }
            else
            {
                try
                {
                    await Share.Default.RequestAsync(new ShareFileRequest
                    {
                        Title = AppResources.ExportComplete_Title,
                        File = new ShareFile(path)
                    }).ConfigureAwait(false);
                    _logger?.LogInformation("Share sheet completed successfully");
                }
                catch (Exception shareEx)
                {
                    _logger?.LogError(shareEx, "Share sheet failed");
                    throw new InvalidOperationException("Failed to share export file.", shareEx);
                }
            }

            return path;
        }
        catch (JsonException jsonEx)
        {
            _logger?.LogError(jsonEx, "JSON serialization failed during export");
            // Clean up file if it was created
            if (path != null && File.Exists(path))
            {
                try { File.Delete(path); } catch { }
            }
            throw new InvalidOperationException("Failed to serialize export data.", jsonEx);
        }
        catch (IOException ioEx)
        {
            _logger?.LogError(ioEx, $"File I/O error during export: {path}");
            throw new InvalidOperationException($"Failed to write export file: {ioEx.Message}", ioEx);
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger?.LogError(ex, "Unexpected error during export");
            throw new InvalidOperationException("An unexpected error occurred during export.", ex);
        }
    }

    public async Task<int> ImportAsync(string filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            var error = "Import file path is null or empty.";
            _logger?.LogError(error);
            throw new ArgumentException(error, nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            var error = $"Export file not found: {filePath}";
            _logger?.LogError(error);
            throw new FileNotFoundException(error, filePath);
        }

        try
        {
            _logger?.LogInformation($"Starting import from: {filePath}");

            var json = await File.ReadAllTextAsync(filePath, ct).ConfigureAwait(false);
            
            if (string.IsNullOrWhiteSpace(json))
            {
                var error = "Import file is empty.";
                _logger?.LogError(error);
                throw new InvalidDataException(error);
            }

            MixExportPackage? pkg;
            try
            {
                pkg = JsonSerializer.Deserialize<MixExportPackage>(json);
            }
            catch (JsonException jsonEx)
            {
                _logger?.LogError(jsonEx, "JSON deserialization failed during import");
                throw new InvalidDataException("Invalid export format: Unable to parse JSON.", jsonEx);
            }

            if (pkg == null)
            {
                var error = "Invalid export format: Deserialization returned null.";
                _logger?.LogError(error);
                throw new InvalidDataException(error);
            }

            _logger?.LogInformation($"Import package scope: {pkg.Scope}, contains {pkg.Mixes?.Count ?? 0} mixes and {pkg.Playlists?.Count ?? 0} playlists");

            // Feature gate checks
            if (pkg.Scope == ExportScope.Shareable && !_features.ShareExportEnabled)
            {
                var error = "Shared imports require Premium or Pro+.";
                _logger?.LogWarning(error);
                throw new InvalidOperationException(error);
            }

            if (pkg.Scope == ExportScope.Personal && !_features.ExportEnabled)
            {
                var error = "Personal imports require Standard+.";
                _logger?.LogWarning(error);
                throw new InvalidOperationException(error);
            }

            var imported = 0;
            var errors = new List<string>();

            // Import mixes
            if (pkg.Mixes?.Count > 0)
            {
                _logger?.LogInformation($"Importing {pkg.Mixes.Count} mixes");
                foreach (var m in pkg.Mixes)
                {
                    try
                    {
                        if (m == null || string.IsNullOrWhiteSpace(m.Name))
                        {
                            _logger?.LogWarning("Skipping mix with null or empty name");
                            errors.Add("Skipped mix with invalid name");
                            continue;
                        }

                        UserPreferences.SaveMix(m);
                        imported++;
                        _logger?.LogDebug($"Successfully imported mix: {m.Name}");
                    }
                    catch (Exception mixEx)
                    {
                        var errorMsg = $"Failed to import mix '{m?.Name ?? "unknown"}': {mixEx.Message}";
                        _logger?.LogError(mixEx, errorMsg);
                        errors.Add(errorMsg);
                    }
                }
            }

            // Import playlists
            if (pkg.Playlists?.Count > 0)
            {
                _logger?.LogInformation($"Importing {pkg.Playlists.Count} playlists");
                foreach (var p in pkg.Playlists)
                {
                    try
                    {
                        if (p == null || string.IsNullOrWhiteSpace(p.Name))
                        {
                            _logger?.LogWarning("Skipping playlist with null or empty name");
                            errors.Add("Skipped playlist with invalid name");
                            continue;
                        }

                        UserPreferences.SavePlaylist(p);
                        imported++;
                        _logger?.LogDebug($"Successfully imported playlist: {p.Name}");
                    }
                    catch (Exception playlistEx)
                    {
                        var errorMsg = $"Failed to import playlist '{p?.Name ?? "unknown"}': {playlistEx.Message}";
                        _logger?.LogError(playlistEx, errorMsg);
                        errors.Add(errorMsg);
                    }
                }
            }

            _logger?.LogInformation($"Import completed: {imported} items imported successfully");

            if (errors.Count > 0)
            {
                _logger?.LogWarning($"Import completed with {errors.Count} errors: {string.Join("; ", errors)}");
                // Consider if you want to throw here or just return partial success
                // For now, we'll return partial success
            }

            if (imported == 0 && (pkg.Mixes?.Count > 0 || pkg.Playlists?.Count > 0))
            {
                throw new InvalidOperationException($"Import failed: No items were successfully imported. Errors: {string.Join("; ", errors)}");
            }

            return imported;
        }
        catch (IOException ioEx)
        {
            _logger?.LogError(ioEx, $"File I/O error during import: {filePath}");
            throw new InvalidOperationException($"Failed to read import file: {ioEx.Message}", ioEx);
        }
        catch (Exception ex) when (ex is not InvalidOperationException && ex is not FileNotFoundException && ex is not ArgumentException && ex is not InvalidDataException)
        {
            _logger?.LogError(ex, "Unexpected error during import");
            throw new InvalidOperationException("An unexpected error occurred during import.", ex);
        }
    }
}