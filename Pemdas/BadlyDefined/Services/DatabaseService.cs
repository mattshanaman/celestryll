using BadlyDefined.Models;
using SQLite;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace BadlyDefined.Services;

/// <summary>
/// Handles all database operations for BadlyDefined game with encryption and comprehensive error handling
/// </summary>
public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly string _dbPath;
    private readonly string _encryptionKey;
    private bool _isInitialized;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private readonly ErrorLoggingService? _errorLogger;

    public DatabaseService(ErrorLoggingService? errorLogger = null)
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "badlydefined.db3");
        _encryptionKey = GenerateOrRetrieveEncryptionKey();
        _errorLogger = errorLogger;

        Debug.WriteLine($"📁 Database path: {_dbPath}");
        Debug.WriteLine($"🔐 Database encryption enabled");
    }

    /// <summary>
    /// Generate or retrieve a device-specific encryption key stored in secure storage
    /// </summary>
    private string GenerateOrRetrieveEncryptionKey()
    {
        try
        {
            const string keyName = "badlydefined_db_key";

            // Try to get existing key with timeout
            var task = SecureStorage.GetAsync(keyName);
            if (!task.Wait(TimeSpan.FromSeconds(2)))
            {
                Debug.WriteLine("⚠️ SecureStorage timeout, using fallback key");
                return GenerateFallbackKey();
            }

            var existingKey = task.Result;

            if (!string.IsNullOrEmpty(existingKey))
            {
                Debug.WriteLine("🔑 Using existing encryption key");
                return existingKey;
            }

            // Generate new 256-bit key
            using var rng = RandomNumberGenerator.Create();
            var keyBytes = new byte[32];
            rng.GetBytes(keyBytes);
            var newKey = Convert.ToBase64String(keyBytes);

            // Try to save with timeout
            var saveTask = SecureStorage.SetAsync(keyName, newKey);
            if (!saveTask.Wait(TimeSpan.FromSeconds(2)))
            {
                Debug.WriteLine("⚠️ SecureStorage save timeout, using generated key without saving");
                return newKey;
            }

            Debug.WriteLine("🔑 New encryption key generated and stored securely");
            return newKey;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"⚠️ Encryption key generation failed: {ex.Message}. Using fallback.");
            return GenerateFallbackKey();
        }
    }

    private string GenerateFallbackKey()
    {
        // Use device-specific identifier as fallback (less secure but functional)
        var deviceId = $"{DeviceInfo.Name}_{DeviceInfo.Manufacturer}_{AppInfo.PackageName}";
        Debug.WriteLine($"🔑 Using fallback key based on device identifier");
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(deviceId.PadRight(32).Substring(0, 32)));
    }

    public async Task InitializeAsync()
    {
        await _initLock.WaitAsync();
        try
        {
            if (_isInitialized && _database != null)
                return;

            Debug.WriteLine("🔧 Initializing BadlyDefined database...");

            // Enable encryption for security
            var options = new SQLiteConnectionString(_dbPath, 
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                storeDateTimeAsTicks: true,
                key: _encryptionKey); // Encryption enabled

            _database = new SQLiteAsyncConnection(options);

            // Validate connection
            await _database.ExecuteScalarAsync<int>("SELECT 1");

            // Create tables with error handling
            await CreateTablesWithRetryAsync();

            // Seed puzzles if needed
            await EnsurePuzzlesSeededAsync();

            // Ensure user progress exists
            await EnsureUserProgressExistsAsync();

            _isInitialized = true;
            Debug.WriteLine("✅ Database initialized successfully with encryption");
        }
        catch (Exception ex)
        {
            var errorMsg = $"Database initialization failed: {ex.Message}";
            Debug.WriteLine($"❌ {errorMsg}");
            await (_errorLogger?.LogErrorAsync(ex, "DatabaseService.InitializeAsync", new Dictionary<string, object>
            {
                { "DbPath", _dbPath },
                { "IsInitialized", _isInitialized }
            }) ?? Task.CompletedTask);
            throw new InvalidOperationException(errorMsg, ex);
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task CreateTablesWithRetryAsync(int maxRetries = 3)
    {
        if (_database == null)
            throw new InvalidOperationException("Database connection is not initialized");

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                await _database.CreateTableAsync<BadDefinition>();
                await _database.CreateTableAsync<UserProgress>();
                await _database.CreateTableAsync<DailyCompletion>();

                Debug.WriteLine("✅ Database tables created successfully");
                return;
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                Debug.WriteLine($"⚠️ Table creation attempt {attempt} failed: {ex.Message}. Retrying...");
                await Task.Delay(TimeSpan.FromMilliseconds(100 * attempt));
            }
        }
    }

    private async Task EnsurePuzzlesSeededAsync()
    {
        if (_database == null)
            throw new InvalidOperationException("Database connection is not initialized");

        try
        {
            var puzzleCount = await _database.Table<BadDefinition>().CountAsync();
            if (puzzleCount == 0)
            {
                Debug.WriteLine("📦 Seeding initial puzzles...");
                await SeedPuzzlesAsync();
            }
            else
            {
                Debug.WriteLine($"📊 Database contains {puzzleCount} puzzles");
            }
        }
        catch (Exception ex)
        {
            var errorMsg = "Failed to check or seed puzzles";
            await (_errorLogger?.LogErrorAsync(ex, "DatabaseService.EnsurePuzzlesSeeded") ?? Task.CompletedTask);
            throw new InvalidOperationException(errorMsg, ex);
        }
    }

    private async Task EnsureUserProgressExistsAsync()
    {
        if (_database == null)
            throw new InvalidOperationException("Database connection is not initialized");

        try
        {
            var progress = await _database.Table<UserProgress>().FirstOrDefaultAsync();
            if (progress == null)
            {
                progress = new UserProgress
                {
                    CurrentStreak = 0,
                    TotalPuzzlesCompleted = 0,
                    TotalPoints = 0,
                    HintTokens = 3,
                    PreferredDifficultySlot = 0,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                };
                await _database.InsertAsync(progress);
                Debug.WriteLine("✅ User progress initialized");
            }
        }
        catch (Exception ex)
        {
            await (_errorLogger?.LogErrorAsync(ex, "DatabaseService.EnsureUserProgressExists") ?? Task.CompletedTask);
            throw;
        }
    }

    public async Task<bool> VerifyDatabaseIntegrity()
    {
        try
        {
            if (_database == null)
                return false;

            var puzzleCount = await _database.Table<BadDefinition>().CountAsync();
            Debug.WriteLine($"📊 Total puzzles in database: {puzzleCount}");

            // Verify we have puzzles for today
            var today = DateTime.UtcNow.Date;
            var todaysPuzzles = await _database.Table<BadDefinition>()
                .Where(p => p.PuzzleDate == today)
                .CountAsync();
            
            Debug.WriteLine($"📅 Today's puzzles: {todaysPuzzles}");

            return puzzleCount > 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Database integrity check failed: {ex.Message}");
            return false;
        }
    }

    // ==================== PUZZLE MANAGEMENT ====================

    public async Task<BadDefinition?> GetTodaysPuzzleAsync(int difficultySlot)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var puzzle = await _database!.Table<BadDefinition>()
                .Where(p => p.PuzzleDate == today && p.DifficultySlot == difficultySlot)
                .FirstOrDefaultAsync();

            if (puzzle != null)
            {
                Debug.WriteLine($"✅ Found puzzle for slot {difficultySlot}: {puzzle.Solution}");
            }
            else
            {
                Debug.WriteLine($"⚠️ No puzzle found for today, slot {difficultySlot}");
            }

            return puzzle;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error getting today's puzzle: {ex.Message}");
            return null;
        }
    }

    public async Task<List<BadDefinition>> GetPuzzlesByDateAsync(DateTime date)
    {
        try
        {
            return await _database!.Table<BadDefinition>()
                .Where(p => p.PuzzleDate == date.Date)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error getting puzzles by date: {ex.Message}");
            return new List<BadDefinition>();
        }
    }

    // ==================== USER PROGRESS ====================

    public async Task<UserProgress> GetUserProgressAsync()
    {
        try
        {
            var progress = await _database!.Table<UserProgress>().FirstOrDefaultAsync();
            if (progress == null)
            {
                progress = new UserProgress
                {
                    CurrentStreak = 0,
                    TotalPuzzlesCompleted = 0,
                    TotalPoints = 0,
                    HintTokens = 3,
                    PreferredDifficultySlot = 0
                };
                await _database.InsertAsync(progress);
            }
            return progress;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error getting user progress: {ex.Message}");
            return new UserProgress();
        }
    }

    public async Task UpdateUserProgressAsync(UserProgress progress)
    {
        try
        {
            await _database!.UpdateAsync(progress);
            Debug.WriteLine("✅ User progress updated");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error updating user progress: {ex.Message}");
        }
    }

    public async Task UpdateStreakAsync()
    {
        try
        {
            var progress = await GetUserProgressAsync();
            var today = DateTime.UtcNow.Date;
            var lastPlayDate = progress.LastPlayDate?.Date;

            if (lastPlayDate == null)
            {
                // First time playing
                progress.CurrentStreak = 1;
                progress.LastPlayDate = today;
            }
            else if (lastPlayDate == today)
            {
                // Already played today, no change
                return;
            }
            else if (lastPlayDate == today.AddDays(-1))
            {
                // Consecutive day
                progress.CurrentStreak++;
                progress.LastPlayDate = today;
            }
            else
            {
                // Streak broken
                progress.CurrentStreak = 1;
                progress.LastPlayDate = today;
            }

            // Update longest streak
            if (progress.CurrentStreak > progress.LongestStreak)
            {
                progress.LongestStreak = progress.CurrentStreak;
            }

            await UpdateUserProgressAsync(progress);
            Debug.WriteLine($"🔥 Streak updated: {progress.CurrentStreak}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error updating streak: {ex.Message}");
        }
    }

    // ==================== COMPLETION TRACKING ====================

    public async Task<bool> RecordCompletionAsync(DailyCompletion completion)
    {
        try
        {
            // Set DateSlotKey for unique constraint
            completion.DateSlotKey = $"{completion.CompletionDate:yyyy-MM-dd}_{completion.DifficultySlot}";

            // Check if already completed
            var existing = await _database!.Table<DailyCompletion>()
                .Where(c => c.DateSlotKey == completion.DateSlotKey)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                Debug.WriteLine($"⚠️ Puzzle already completed: {completion.DateSlotKey}");
                return false;
            }

            // Insert completion
            await _database.InsertAsync(completion);

            // Update user progress
            var progress = await GetUserProgressAsync();
            progress.TotalPuzzlesCompleted++;
            progress.TotalPoints += completion.PointsEarned;

            // Update difficulty-specific counts
            switch (completion.DifficultySlot)
            {
                case 0: progress.EasyCompleted++; break;
                case 1: progress.MediumCompleted++; break;
                case 2: progress.HardCompleted++; break;
            }

            // Update average attempts
            progress.AverageAttempts = 
                (progress.AverageAttempts * (progress.TotalPuzzlesCompleted - 1) + completion.AttemptsCount) 
                / progress.TotalPuzzlesCompleted;

            // Update best attempts
            if (completion.AttemptsCount < progress.BestAttempts)
            {
                progress.BestAttempts = completion.AttemptsCount;
            }

            await UpdateUserProgressAsync(progress);

            // Update streak
            await UpdateStreakAsync();

            Debug.WriteLine($"✅ Completion recorded: {completion.PuzzleIdentifier}, {completion.PointsEarned} points");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error recording completion: {ex.Message}");
            return false;
        }
    }

    public async Task<List<int>> GetTodaysCompletedDifficultiesAsync()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var completions = await _database!.Table<DailyCompletion>()
                .Where(c => c.CompletionDate == today)
                .ToListAsync();

            return completions.Select(c => c.DifficultySlot).ToList();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error getting completed difficulties: {ex.Message}");
            return new List<int>();
        }
    }

    public async Task<bool> HasCompletedDifficultyTodayAsync(int difficultySlot)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var dateSlotKey = $"{today:yyyy-MM-dd}_{difficultySlot}";
            
            var completion = await _database!.Table<DailyCompletion>()
                .Where(c => c.DateSlotKey == dateSlotKey)
                .FirstOrDefaultAsync();

            return completion != null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error checking completion: {ex.Message}");
            return false;
        }
    }

    public async Task<int> GetTotalPointsEarnedAsync()
    {
        try
        {
            var progress = await GetUserProgressAsync();
            return progress.TotalPoints;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error getting total points: {ex.Message}");
            return 0;
        }
    }

    public async Task<DailyCompletion?> GetCompletionStatsAsync(int difficultySlot, DateTime date)
    {
        try
        {
            var dateSlotKey = $"{date:yyyy-MM-dd}_{difficultySlot}";
            return await _database!.Table<DailyCompletion>()
                .Where(c => c.DateSlotKey == dateSlotKey)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error getting completion stats: {ex.Message}");
            return null;
        }
    }

    // ==================== PUZZLE SEEDING ====================

    private async Task SeedPuzzlesAsync()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var puzzles = new List<BadDefinition>();

            // Seed puzzles for next 2 years (730 days) across all difficulties
            for (int dayOffset = 0; dayOffset < 730; dayOffset++)
            {
                var date = today.AddDays(dayOffset);

                // Easy puzzle
                var easyPuzzle = Data.PuzzleLibrary.GetDailyPuzzle(date, 0);
                if (easyPuzzle.HasValue)
                {
                    puzzles.Add(CreatePuzzleDefinition(date, 0, easyPuzzle.Value));
                }

                // Medium puzzle
                var mediumPuzzle = Data.PuzzleLibrary.GetDailyPuzzle(date, 1);
                if (mediumPuzzle.HasValue)
                {
                    puzzles.Add(CreatePuzzleDefinition(date, 1, mediumPuzzle.Value));
                }

                // Hard puzzle
                var hardPuzzle = Data.PuzzleLibrary.GetDailyPuzzle(date, 2);
                if (hardPuzzle.HasValue)
                {
                    puzzles.Add(CreatePuzzleDefinition(date, 2, hardPuzzle.Value));
                }
            }

            await _database!.InsertAllAsync(puzzles);
            Debug.WriteLine($"✅ Seeded {puzzles.Count} puzzles from library (730 days × 3 difficulties = 2 years)");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error seeding puzzles: {ex.Message}");
            throw;
        }
    }

    private BadDefinition CreatePuzzleDefinition(
        DateTime date, 
        int difficultySlot,
        (string solution, string definition, string category) puzzleData)
    {
        var (solution, definition, category) = puzzleData;
        var letterCount = solution.Replace(" ", "").Replace("-", "").Length;

        // Calculate hint letter indices
        var hintIndices = GenerateHintIndices(solution);

        var difficultyCode = difficultySlot switch
        {
            0 => "E",
            1 => "M",
            2 => "H",
            _ => "?"
        };

        return new BadDefinition
        {
            PuzzleDate = date,
            PuzzleIdentifier = $"BD{date:yyyyMMdd}-{difficultyCode}",
            DifficultySlot = difficultySlot,
            Difficulty = (DifficultyLevel)difficultySlot,
            Solution = solution,
            BadDefinitionText = definition,
            Category = category,
            LetterCount = letterCount,
            BasePoints = difficultySlot switch
            {
                0 => 100,
                1 => 200,
                2 => 300,
                _ => 100
            },
            FirstHintLetters = hintIndices.first,
            SecondHintLetters = hintIndices.second,
            ThirdHintLetters = hintIndices.third,
            MaxAttempts = difficultySlot switch
            {
                0 => 5,
                1 => 6,
                2 => 7,
                _ => 5
            }
        };
    }

    private (string first, string second, string third) GenerateHintIndices(string solution)
    {
        // Get letter indices (skip spaces and hyphens)
        var letterIndices = new List<int>();
        for (int i = 0; i < solution.Length; i++)
        {
            if (solution[i] != ' ' && solution[i] != '-')
            {
                letterIndices.Add(i);
            }
        }

        if (letterIndices.Count == 0)
            return ("", "", "");

        // First hint: First letter
        var firstHint = letterIndices[0].ToString();

        // Second hint: Last letter or second letter
        var secondHint = letterIndices.Count > 1 
            ? letterIndices[^1].ToString() 
            : "";

        // Third hint: Middle letter(s)
        var thirdHint = "";
        if (letterIndices.Count > 3)
        {
            var middleIndices = letterIndices.Skip(1).Take(letterIndices.Count - 2).ToList();
            if (middleIndices.Any())
            {
                var revealCount = Math.Max(1, middleIndices.Count / 3);
                thirdHint = string.Join(",", middleIndices.Take(revealCount));
            }
        }

        return (firstHint, secondHint, thirdHint);
    }

    // ==================== CLEANUP ====================

    public async Task ClearAndRegeneratePuzzlesAsync()
    {
        try
        {
            Debug.WriteLine("🗑️ Clearing all puzzles...");
            await _database!.DeleteAllAsync<BadDefinition>();
            await _database.DeleteAllAsync<DailyCompletion>();
            
            Debug.WriteLine("📦 Regenerating puzzles...");
            await SeedPuzzlesAsync();
            
            Debug.WriteLine("✅ Puzzles regenerated successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error regenerating puzzles: {ex.Message}");
            throw;
        }
    }
}
