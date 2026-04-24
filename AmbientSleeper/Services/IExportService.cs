using AmbientSleeper.Models;

namespace AmbientSleeper.Services;

public interface IExportService
{
    Task<string> ExportAsync(ExportScope scope, bool shareViaEmail = false, CancellationToken ct = default);
    Task<int> ImportAsync(string filePath, CancellationToken ct = default);
}