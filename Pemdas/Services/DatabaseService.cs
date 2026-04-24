using Pemdas.Models;
using SQLite;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Security.Cryptography;

namespace Pemdas.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private readonly ErrorLoggingService? _errorLogger;
    private readonly string _encryptionKey;
    private const string DatabaseFilename = "pemdas.db3";

    // Performance: Cache for frequently accessed data
    private UserProgress? _cachedUserProgress;
    private DateTime _userProgressCacheTime;
    private readonly TimeSpan _cacheTimeout = TimeSpan.FromMinutes(5);

    // Performance: Cache today's puzzle to avoid repeated DB queries
    private DailyPuzzle? _cachedTodaysPuzzle;
    private DateTime _todaysPuzzleCacheDate;

    public DatabaseService(ErrorLoggingService? errorLogger = null)
    {
        _errorLogger = errorLogger;
        _encryptionKey = GenerateOrRetrieveEncryptionKey();
        System.Diagnostics.Debug.WriteLine($"?? Database: {Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename)}");
        System.Diagnostics.Debug.WriteLine("?? Database encryption enabled");
    }

    /// <summary>
    /// Generate or retrieve a device-specific encryption key stored in secure storage
    /// </summary>
    private string GenerateOrRetrieveEncryptionKey()
    {
        try
        {
            const string keyName = "pemdas_db_key";
            var existingKey = SecureStorage.GetAsync(keyName).Result;

            if (!string.IsNullOrEmpty(existingKey))
            {
                return existingKey;
            }

            // Generate new 256-bit key
            using var rng = RandomNumberGenerator.Create();
            var keyBytes = new byte[32];
            rng.GetBytes(keyBytes);
            var newKey = Convert.ToBase64String(keyBytes);

            SecureStorage.SetAsync(keyName, newKey).Wait();
            System.Diagnostics.Debug.WriteLine("?? New encryption key generated and stored securely");

            return newKey;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"?? Encryption key generation failed: {ex.Message}. Using fallback.");
            // Fallback to device-specific identifier (less secure but functional)
            return $"{DeviceInfo.Name}_{DeviceInfo.Manufacturer}_{AppInfo.PackageName}";
        }
    }

    public async Task Init()
    {
        if (_database is not null)
            return;

        await _initLock.WaitAsync();
        try
        {
            if (_database is not null)
                return;

            System.Diagnostics.Debug.WriteLine("?? Initializing Pemdas database...");

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

            // Performance + Security: Enable Write-Ahead Logging with encryption
            var options = new SQLiteConnectionString(
                databasePath,
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                storeDateTimeAsTicks: true,
                key: _encryptionKey);

            _database = new SQLiteAsyncConnection(options);

            // Validate connection
            await _database.ExecuteScalarAsync<int>("SELECT 1");

            await CreateTablesAsync();
            await EnsureUserProgressExistsAsync();
            await EnsurePuzzlesExistAsync();

            System.Diagnostics.Debug.WriteLine("? Database initialized successfully with encryption");
        }
        catch (Exception ex)
        {
            var errorMsg = $"Database initialization failed: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"? {errorMsg}");
            await (_errorLogger?.LogErrorAsync(ex, "DatabaseService.Init", new Dictionary<string, object>
            {
                { "DatabasePath", Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename) }
            }) ?? Task.CompletedTask);
            throw new InvalidOperationException(errorMsg, ex);
        }
        finally
        {
            _initLock.Release();
        }
    }

    private async Task CreateTablesAsync()
    {
        try
        {
            // Performance: Create tables with optimized settings
            await _database!.CreateTableAsync<DailyPuzzle>();
            await _database.CreateTableAsync<UserProgress>();
            await _database.CreateTableAsync<PuzzleAttempt>();
            await _database.CreateTableAsync<DailyCompletion>();

            // Performance: Create indexes for frequently queried columns
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_puzzle_date ON DailyPuzzles(PuzzleDate)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_puzzle_identifier ON DailyPuzzles(PuzzleIdentifier)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_attempt_puzzle_id ON PuzzleAttempts(PuzzleId)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_attempt_date ON PuzzleAttempts(AttemptDate)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_completion_date ON DailyCompletions(CompletionDate)");
            await _database.ExecuteAsync("CREATE INDEX IF NOT EXISTS idx_completion_dateslot ON DailyCompletions(DateSlotKey)");

            System.Diagnostics.Debug.WriteLine("? Database tables and indexes created");
        }
        catch (Exception ex)
        {
            await (_errorLogger?.LogErrorAsync(ex, "DatabaseService.CreateTables") ?? Task.CompletedTask);
            throw;
        }
    }

    private async Task EnsureUserProgressExistsAsync()
    {
        try
        {
            var userProgress = await GetUserProgressInternal();
            if (userProgress == null)
            {
                var newProgress = new UserProgress
                {
                    CurrentStreak = 0,
                    LongestStreak = 0,
                    HintTokens = 3,
                    LastPlayedDate = DateTime.MinValue,
                    TotalPuzzlesSolved = 0,
                    TotalPoints = 0,
                    IsSubscribed = false,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                };
                await _database!.InsertAsync(newProgress);

                // Performance: Initialize cache
                _cachedUserProgress = newProgress;
                _userProgressCacheTime = DateTime.UtcNow;

                System.Diagnostics.Debug.WriteLine("? User progress initialized");
            }
        }
        catch (Exception ex)
        {
            await (_errorLogger?.LogErrorAsync(ex, "DatabaseService.EnsureUserProgressExists") ?? Task.CompletedTask);
            throw;
        }
    }

    private async Task EnsurePuzzlesExistAsync()
    {
        var count = await _database!.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM DailyPuzzles");
        
        System.Diagnostics.Debug.WriteLine($"?? Database Check: {count} puzzles found (Expected: 43,800)");
        
        // Check if database is corrupted (wrong count or wrong assignments)
        var today = DateTime.UtcNow.Date;
        var todayCount = await _database.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM DailyPuzzles WHERE PuzzleDate = ?", today);
        
        System.Diagnostics.Debug.WriteLine($"?? Today's puzzles: {todayCount}/8 available");
        
        // If incomplete or wrong assignments, regenerate
        if (count == 0)
        {
            System.Diagnostics.Debug.WriteLine("?? No puzzles found! Generating 43,800 puzzles...");
            await InitializePuzzles();
        }
        else if (count < 43800 || todayCount != 8)
        {
            System.Diagnostics.Debug.WriteLine($"?? Database corrupted! Found {count}/43,800. Regenerating...");
            
            // Check if slots are assigned correctly
            if (todayCount > 0)
            {
                var firstPuzzle = await _database.Table<DailyPuzzle>()
                    .Where(p => p.PuzzleDate == today && p.DifficultySlot == 0)
                    .FirstOrDefaultAsync();
                
                if (firstPuzzle != null && firstPuzzle.Difficulty != DifficultyLevel.Easy)
                {
                    System.Diagnostics.Debug.WriteLine($"? CRITICAL: Slot 0 has {firstPuzzle.Difficulty} instead of Easy!");
                    System.Diagnostics.Debug.WriteLine("?? Clearing and regenerating database...");
                    
                    await _database.ExecuteAsync("DELETE FROM DailyPuzzles");
                    count = 0;
                }
            }
            
            if (count == 0 || count < 43800)
            {
                await InitializePuzzles();
            }
        }
        
        // Final verification
        var finalCount = await _database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM DailyPuzzles");
        var finalTodayCount = await _database.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM DailyPuzzles WHERE PuzzleDate = ?", today);
        
        if (finalCount < 43800 || finalTodayCount != 8)
        {
            System.Diagnostics.Debug.WriteLine("? CRITICAL: Database verification FAILED after regeneration!");
            System.Diagnostics.Debug.WriteLine($"   Total: {finalCount}/43,800");
            System.Diagnostics.Debug.WriteLine($"   Today: {finalTodayCount}/8");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"? Database verified: {finalCount} puzzles, {finalTodayCount} for today");
        }
    }
    
    public async Task<bool> VerifyDatabaseIntegrity()
    {
        try
        {
            await Init();
            
            System.Diagnostics.Debug.WriteLine("?? VERIFYING DATABASE INTEGRITY");
            System.Diagnostics.Debug.WriteLine("================================");
            
            var total = await _database!.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM DailyPuzzles");
            System.Diagnostics.Debug.WriteLine($"Total Puzzles: {total} (Expected: 43,800)");
            
            var today = DateTime.UtcNow.Date;
            var todayCount = await _database.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM DailyPuzzles WHERE PuzzleDate = ?", today);
            System.Diagnostics.Debug.WriteLine($"Today's Puzzles: {todayCount} (Expected: 8)");
            
            if (total > 0)
            {
                var minDate = await _database.ExecuteScalarAsync<DateTime>("SELECT MIN(PuzzleDate) FROM DailyPuzzles");
                var maxDate = await _database.ExecuteScalarAsync<DateTime>("SELECT MAX(PuzzleDate) FROM DailyPuzzles");
                System.Diagnostics.Debug.WriteLine($"Date Range: {minDate:yyyy-MM-dd} to {maxDate:yyyy-MM-dd}");
            }
            
            if (todayCount > 0)
            {
                var slots = await _database.QueryAsync<DailyPuzzle>(
                    "SELECT DifficultySlot, Difficulty, Mode FROM DailyPuzzles WHERE PuzzleDate = ? ORDER BY DifficultySlot", 
                    today);
                
                System.Diagnostics.Debug.WriteLine("Available Today:");
                foreach (var p in slots)
                {
                    System.Diagnostics.Debug.WriteLine($"  Slot {p.DifficultySlot}: {p.Difficulty} - {p.Mode}");
                }
            }
            
            System.Diagnostics.Debug.WriteLine("================================");
            
            return total > 0 && todayCount == 8;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? Error verifying database: {ex.Message}");
            return false;
        }
    }

    public async Task<DailyPuzzle?> GetTodaysPuzzle()
    {
        try
        {
            await Init();
            var today = DateTime.UtcNow.Date;
            
            System.Diagnostics.Debug.WriteLine($"?? GetTodaysPuzzle called for date: {today:yyyy-MM-dd}");
            
            // Get user's preferred difficulty
            var progress = await GetUserProgress();
            var difficultySlot = progress?.PreferredDifficultySlot ?? 0; // Default to Easy
            
            System.Diagnostics.Debug.WriteLine($"?? Looking for difficulty slot: {difficultySlot}");
            
            // Performance: Return cached puzzle if still valid and same difficulty
            if (_cachedTodaysPuzzle != null && _todaysPuzzleCacheDate == today && _cachedTodaysPuzzle.DifficultySlot == difficultySlot)
            {
                System.Diagnostics.Debug.WriteLine($"? Returning cached puzzle (slot {difficultySlot})");
                return _cachedTodaysPuzzle;
            }
            
            var puzzle = await _database!.Table<DailyPuzzle>()
                .Where(p => p.PuzzleDate == today && p.DifficultySlot == difficultySlot)
                .FirstOrDefaultAsync();
            
            if (puzzle != null)
            {
                System.Diagnostics.Debug.WriteLine($"? Found puzzle: {puzzle.Difficulty} - {puzzle.Mode}");
                
                // Performance: Cache the result
                _cachedTodaysPuzzle = puzzle;
                _todaysPuzzleCacheDate = today;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"? NO PUZZLE FOUND for date {today:yyyy-MM-dd}, slot {difficultySlot}");
                
                // Diagnostic: Check if ANY puzzles exist for today
                var anyTodayPuzzles = await _database.Table<DailyPuzzle>()
                    .Where(p => p.PuzzleDate == today)
                    .CountAsync();
                System.Diagnostics.Debug.WriteLine($"   Puzzles available today: {anyTodayPuzzles}/8");
                
                if (anyTodayPuzzles == 0)
                {
                    System.Diagnostics.Debug.WriteLine($"   ?? No puzzles at all for today! Database may be empty or dates misaligned.");
                    
                    // Check total puzzle count
                    var totalCount = await _database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM DailyPuzzles");
                    System.Diagnostics.Debug.WriteLine($"   Total puzzles in DB: {totalCount}");
                    
                    // Check date range
                    if (totalCount > 0)
                    {
                        var minDate = await _database.ExecuteScalarAsync<DateTime>("SELECT MIN(PuzzleDate) FROM DailyPuzzles");
                        var maxDate = await _database.ExecuteScalarAsync<DateTime>("SELECT MAX(PuzzleDate) FROM DailyPuzzles");
                        System.Diagnostics.Debug.WriteLine($"   Date range in DB: {minDate:yyyy-MM-dd} to {maxDate:yyyy-MM-dd}");
                        System.Diagnostics.Debug.WriteLine($"   Today is: {today:yyyy-MM-dd}");
                    }
                }
                else
                {
                    // Fallback: Try to get Easy difficulty (slot 0) if preferred slot doesn't exist
                    if (difficultySlot != 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"   ?? Trying fallback to Easy (slot 0)...");
                        puzzle = await _database.Table<DailyPuzzle>()
                            .Where(p => p.PuzzleDate == today && p.DifficultySlot == 0)
                            .FirstOrDefaultAsync();
                        
                        if (puzzle != null)
                        {
                            System.Diagnostics.Debug.WriteLine($"   ? Fallback successful! Using Easy difficulty.");
                            
                            // Update user's preference to Easy since their preferred slot doesn't exist
                            if (progress != null)
                            {
                                progress.PreferredDifficultySlot = 0;
                                await UpdateUserProgress(progress);
                            }
                            
                            _cachedTodaysPuzzle = puzzle;
                            _todaysPuzzleCacheDate = today;
                        }
                    }
                    
                    if (puzzle == null)
                    {
                        // Show what slots ARE available
                        var availableSlots = await _database.QueryAsync<DailyPuzzle>(
                            "SELECT DifficultySlot, Difficulty FROM DailyPuzzles WHERE PuzzleDate = ? ORDER BY DifficultySlot", 
                            today);
                        System.Diagnostics.Debug.WriteLine($"   Available slots: {string.Join(", ", availableSlots.Select(p => $"{p.DifficultySlot}={p.Difficulty}"))}");
                    }
                }
            }
            
            return puzzle;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? Error getting today's puzzle: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
            return null;
        }
    }
    
    public async Task<DailyPuzzle?> GetTodaysPuzzleByDifficulty(int difficultySlot)
    {
        try
        {
            await Init();
            var today = DateTime.UtcNow.Date;
            
            var puzzle = await _database!.Table<DailyPuzzle>()
                .Where(p => p.PuzzleDate == today && p.DifficultySlot == difficultySlot)
                .FirstOrDefaultAsync();
            
            return puzzle;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting today's puzzle by difficulty: {ex.Message}");
            return null;
        }
    }
    
    public async Task<List<DailyPuzzle>> GetAllTodaysPuzzles()
    {
        try
        {
            await Init();
            var today = DateTime.UtcNow.Date;
            
            var puzzles = await _database!.Table<DailyPuzzle>()
                .Where(p => p.PuzzleDate == today)
                .OrderBy(p => p.DifficultySlot)
                .ToListAsync();
            
            return puzzles;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting all today's puzzles: {ex.Message}");
            return new List<DailyPuzzle>();
        }
    }

    public async Task<List<DailyPuzzle>> GetPuzzleArchive(int page = 0, int pageSize = 50)
    {
        try
        {
            await Init();
            var today = DateTime.UtcNow.Date;
            
            // Performance: Implement pagination to reduce memory usage
            return await _database!.Table<DailyPuzzle>()
                .Where(p => p.PuzzleDate < today)
                .OrderByDescending(p => p.PuzzleDate)
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting puzzle archive: {ex.Message}");
            return new List<DailyPuzzle>();
        }
    }

    public async Task<UserProgress?> GetUserProgress()
    {
        try
        {
            await Init();
            
            // Performance: Return cached data if still fresh
            if (_cachedUserProgress != null && 
                DateTime.UtcNow - _userProgressCacheTime < _cacheTimeout)
            {
                return _cachedUserProgress;
            }
            
            var progress = await GetUserProgressInternal();
            
            // Performance: Update cache
            if (progress != null)
            {
                _cachedUserProgress = progress;
                _userProgressCacheTime = DateTime.UtcNow;
            }
            
            return progress;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting user progress: {ex.Message}");
            return null;
        }
    }

    private async Task<UserProgress?> GetUserProgressInternal()
    {
        return await _database!.Table<UserProgress>().FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateUserProgress(UserProgress progress)
    {
        if (progress == null)
        {
            System.Diagnostics.Debug.WriteLine("Cannot update null user progress");
            return false;
        }

        try
        {
            await Init();
            await _database!.UpdateAsync(progress);
            
            // Performance: Invalidate cache
            _cachedUserProgress = progress;
            _userProgressCacheTime = DateTime.UtcNow;
            
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating user progress: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> SavePuzzleAttempt(PuzzleAttempt attempt)
    {
        if (attempt == null)
        {
            System.Diagnostics.Debug.WriteLine("Cannot save null puzzle attempt");
            return false;
        }

        try
        {
            await Init();
            await _database!.InsertAsync(attempt);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving puzzle attempt: {ex.Message}");
            return false;
        }
    }

    public async Task<PuzzleAttempt?> GetTodaysAttempt()
    {
        try
        {
            await Init();
            var todaysPuzzle = await GetTodaysPuzzle();
            if (todaysPuzzle == null)
                return null;

            // Performance: Direct query instead of loading all attempts
            return await _database!.Table<PuzzleAttempt>()
                .Where(a => a.PuzzleId == todaysPuzzle.Id)
                .OrderByDescending(a => a.AttemptDate)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting today's attempt: {ex.Message}");
            return null;
        }
    }
    
    public async Task<List<PuzzleAttempt>> GetAllTodaysAttempts()
    {
        try
        {
            await Init();
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);
            
            var attempts = await _database!.Table<PuzzleAttempt>()
                .Where(a => a.AttemptDate >= today && a.AttemptDate < tomorrow)
                .ToListAsync();
            
            return attempts ?? new List<PuzzleAttempt>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting all today's attempts: {ex.Message}");
            return new List<PuzzleAttempt>();
        }
    }

    public async Task<List<PuzzleAttempt>> GetLeaderboard(int puzzleId, int limit = 100)
    {
        try
        {
            await Init();
            return await _database!.Table<PuzzleAttempt>()
                .Where(a => a.PuzzleId == puzzleId && a.Solved)
                .OrderBy(a => a.TimeSpent)
                .Take(limit)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting leaderboard: {ex.Message}");
            return new List<PuzzleAttempt>();
        }
    }

    private async Task InitializePuzzles()
    {
        try
        {
            // Use a fixed reference date so all users get the same puzzles
            var referenceDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var startDate = DateTime.UtcNow.Date;
            const int batchSize = 100;
            const int totalDays = 5475; // 15 years of puzzles

            System.Diagnostics.Debug.WriteLine($"Generating {totalDays} days × 8 difficulties = {totalDays * 8} total puzzles");

            // Performance: Use transaction for bulk inserts
            await _database!.RunInTransactionAsync(tran =>
            {
                for (int batch = 0; batch < totalDays; batch += batchSize)
                {
                    var count = Math.Min(batchSize, totalDays - batch);
                    var puzzles = new List<DailyPuzzle>(count * 8); // 8 difficulties per day

                    for (int day = 0; day < count; day++)
                    {
                        var actualDay = batch + day;
                        var puzzleDate = startDate.AddDays(actualDay);
                        
                        // Generate all 8 difficulty levels for this day
                        puzzles.AddRange(GenerateAllDifficultiesForDay(puzzleDate, referenceDate));
                    }

                    tran.InsertAll(puzzles);
                    System.Diagnostics.Debug.WriteLine($"Inserted batch {batch / batchSize + 1} of {(totalDays + batchSize - 1) / batchSize} ({puzzles.Count} puzzles)");
                }
            });
            
            System.Diagnostics.Debug.WriteLine($"? Generated {totalDays * 8} total puzzles (15 years × 8 difficulties)");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing puzzles: {ex.Message}");
            throw;
        }
    }

    private DailyPuzzle GeneratePuzzleForDay(DateTime puzzleDate, DateTime referenceDate)
    {
        // Calculate days since reference date to create consistent seed across all users
        var daysSinceReference = (int)(puzzleDate - referenceDate).TotalDays;
        
        // Use the date itself to determine week day pattern
        var weekDay = daysSinceReference % 7;
        var mode = (weekDay & 1) == 0 ? PuzzleMode.SolveIt : PuzzleMode.BuildIt;

        var difficulty = weekDay switch
        {
            0 => DifficultyLevel.Easy,
            1 => DifficultyLevel.Medium,
            2 => DifficultyLevel.Hard,
            3 => DifficultyLevel.Creative,
            4 => DifficultyLevel.Tricky,
            5 => DifficultyLevel.Speed,
            6 => DifficultyLevel.Boss,
            _ => DifficultyLevel.Easy
        };

        // Use days since reference as seed for deterministic puzzle generation
        var (puzzleData, solution, hint, points, timeLimit) = mode == PuzzleMode.SolveIt
            ? GenerateSolveItPuzzle(difficulty, daysSinceReference)
            : GenerateBuildItPuzzle(difficulty, daysSinceReference);

        return new DailyPuzzle
        {
            PuzzleDate = puzzleDate,
            DifficultySlot = 0, // Default slot (kept for backwards compatibility)
            Mode = mode,
            Difficulty = difficulty,
            PuzzleData = puzzleData,
            Solution = solution,
            Hint = hint,
            BasePoints = points,
            TimeLimit = timeLimit
        };
    }
    
    private List<DailyPuzzle> GenerateAllDifficultiesForDay(DateTime puzzleDate, DateTime referenceDate)
    {
        // Generate one puzzle for each difficulty level
        var daysSinceReference = (int)(puzzleDate - referenceDate).TotalDays;
        var puzzles = new List<DailyPuzzle>(8);
        
        // Define all difficulty levels
        var difficulties = new[]
        {
            DifficultyLevel.Easy,
            DifficultyLevel.Medium,
            DifficultyLevel.Hard,
            DifficultyLevel.Creative,
            DifficultyLevel.Tricky,
            DifficultyLevel.Speed,
            DifficultyLevel.Boss,
            DifficultyLevel.Expert
        };
        
        for (int slot = 0; slot < difficulties.Length; slot++)
        {
            var difficulty = difficulties[slot];
            
            // Determine mode based on difficulty
            var mode = difficulty switch
            {
                DifficultyLevel.Medium or DifficultyLevel.Creative or DifficultyLevel.Speed => PuzzleMode.BuildIt,
                _ => PuzzleMode.SolveIt
            };
            
            // Use day + slot as seed for variety within the same day
            var seed = daysSinceReference * 8 + slot;
            
            var (puzzleData, solution, hint, points, timeLimit) = mode == PuzzleMode.SolveIt
                ? GenerateSolveItPuzzle(difficulty, seed)
                : GenerateBuildItPuzzle(difficulty, seed);
            
            // Generate unique puzzle identifier: "P20240115-E" (Puzzle + Date + Difficulty initial)
            var difficultyCode = difficulty switch
            {
                DifficultyLevel.Easy => "E",
                DifficultyLevel.Medium => "M",
                DifficultyLevel.Hard => "H",
                DifficultyLevel.Creative => "C",
                DifficultyLevel.Tricky => "T",
                DifficultyLevel.Speed => "S",
                DifficultyLevel.Boss => "B",
                DifficultyLevel.Expert => "X",
                _ => "E"
            };
            var puzzleId = $"P{puzzleDate:yyyyMMdd}-{difficultyCode}";
            
            puzzles.Add(new DailyPuzzle
            {
                PuzzleDate = puzzleDate,
                PuzzleIdentifier = puzzleId,
                DifficultySlot = slot,
                Mode = mode,
                Difficulty = difficulty,
                PuzzleData = puzzleData,
                Solution = solution,
                Hint = hint,
                BasePoints = points,
                TimeLimit = timeLimit
            });
        }
        
        return puzzles;
    }

    private (string puzzleData, string solution, string hint, int points, int timeLimit) GenerateSolveItPuzzle(DifficultyLevel difficulty, int seed)
    {
        try
        {
            var random = new Random(seed);

            return difficulty switch
            {
                DifficultyLevel.Easy => GenerateEasySolveIt(random),
                DifficultyLevel.Hard => GenerateHardSolveIt(random),
                DifficultyLevel.Tricky => GenerateTrickySolveIt(random),
                DifficultyLevel.Boss => GenerateBossSolveIt(random),
                DifficultyLevel.Expert => GenerateExpertSolveIt(random),
                _ => GenerateEasySolveIt(random)
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error generating Solve It puzzle: {ex.Message}");
            return GenerateEasySolveIt(new Random(seed));
        }
    }

    private (string puzzleData, string solution, string hint, int points, int timeLimit) GenerateEasySolveIt(Random random)
    {
        // Generate different PEMDAS-focused puzzle types
        var puzzleType = random.Next(0, 3);
        
        if (puzzleType == 0)
        {
            // Type A: Multiplication First (e.g., ? × 4 + 3 = 19)
            var answer = random.Next(2, 10);
            var multiplier = random.Next(2, 5);
            var addend = random.Next(1, 10);
            var result = (answer * multiplier) + addend;

            var puzzle = new SolveItPuzzle
            {
                Equation = $"? × {multiplier} + {addend} = {result}",
                BlankPositions = [0],
                Solutions = [answer],
                AllowsExponents = false
            };

            return (
                JsonSerializer.Serialize(puzzle),
                answer.ToString(),
                "Remember PEMDAS: Multiply before adding! Do ? × {multiplier} first.",
                100,
                0
            );
        }
        else if (puzzleType == 1)
        {
            // Type B: Division First (e.g., ? ÷ 2 + 5 = 9)
            var answer = random.Next(4, 20) * 2; // Ensure even for clean division
            var divisor = 2;
            var addend = random.Next(1, 10);
            var result = (answer / divisor) + addend;

            var puzzle = new SolveItPuzzle
            {
                Equation = $"? ÷ {divisor} + {addend} = {result}",
                BlankPositions = [0],
                Solutions = [answer],
                AllowsExponents = false
            };

            return (
                JsonSerializer.Serialize(puzzle),
                answer.ToString(),
                "Remember PEMDAS: Divide before adding! Do ? ÷ {divisor} first.",
                100,
                0
            );
        }
        else
        {
            // Type C: Addition with multiplication (e.g., ? + 2 × 3 = 13)
            var answer = random.Next(2, 15);
            var multiplier1 = random.Next(2, 5);
            var multiplier2 = random.Next(2, 5);
            var result = answer + (multiplier1 * multiplier2);

            var puzzle = new SolveItPuzzle
            {
                Equation = $"? + {multiplier1} × {multiplier2} = {result}",
                BlankPositions = [0],
                Solutions = [answer],
                AllowsExponents = false
            };

            return (
                JsonSerializer.Serialize(puzzle),
                answer.ToString(),
                "Remember PEMDAS: Do {multiplier1} × {multiplier2} first, THEN add. Don't work left-to-right!",
                100,
                0
            );
        }
    }

    private (string puzzleData, string solution, string hint, int points, int timeLimit) GenerateHardSolveIt(Random random)
    {
        // Remove unnecessary parentheses - teach natural PEMDAS
        var x = random.Next(2, 15);
        var y = random.Next(1, 10) * 2; // Always even for integer division
        
        // Ensure x and y are different
        while (y == x)
        {
            y = random.Next(1, 10) * 2;
        }
        
        // Natural PEMDAS: A × 2 + B ÷ 2 = result (no parentheses needed!)
        var result = (x * 2) + (y / 2);

        var puzzle = new SolveItPuzzle
        {
            Equation = $"A × 2 + B ÷ 2 = {result}",
            BlankPositions = [0, 1],
            Solutions = [x, y],
            AllowsExponents = false
        };

        return (
            JsonSerializer.Serialize(puzzle),
            $"{x}, {y}",
            "Remember PEMDAS: Do A × 2 and B ÷ 2 first (left-to-right), then add. A and B must be different. B must be even. Negative values are allowed!",
            300,
            0
        );
    }

    private (string puzzleData, string solution, string hint, int points, int timeLimit) GenerateTrickySolveIt(Random random)
    {
        // Generate PEMDAS-focused puzzle with mixed operations
        var puzzleType = random.Next(0, 2);
        
        if (puzzleType == 0)
        {
            // Type A: Mixed Add/Multiply (e.g., A + B × C = 30)
            var b = random.Next(2, 6);
            var c = random.Next(2, 6);
            var productBC = b * c;
            var a = random.Next(productBC + 2, productBC + 20);
            
            // Ensure all three are different
            while (b == a)
            {
                b = random.Next(2, 6);
                productBC = b * c;
            }
            while (c == a || c == b)
            {
                c = random.Next(2, 6);
                productBC = b * c;
            }
            
            var result = a + productBC;

            var puzzle = new SolveItPuzzle
            {
                Equation = $"A + B × C = {result}",
                BlankPositions = [0, 1, 2],
                Solutions = [a, b, c],
                AllowsExponents = false
            };

            return (
                JsonSerializer.Serialize(puzzle),
                $"{a}, {b}, {c}",
                "Remember PEMDAS: Do B × C first, then add A. All three values must be different integers. Negative values are allowed!",
                400,
                0
            );
        }
        else
        {
            // Type B: Division before subtraction (e.g., A - B ÷ C = result)
            var b = random.Next(4, 20);
            var c = random.Next(2, 5);
            
            // Ensure B is divisible by C for integer result
            b = (b / c) * c;
            
            var quotientBC = b / c;
            var a = quotientBC + random.Next(2, 10);
            
            // Ensure all three are different
            while (b == a)
            {
                a = quotientBC + random.Next(2, 10);
            }
            while (c == a || c == b)
            {
                c = random.Next(2, 5);
                b = (b / c) * c;
                quotientBC = b / c;
            }
            
            var result = a - quotientBC;

            var puzzle = new SolveItPuzzle
            {
                Equation = $"A - B ÷ C = {result}",
                BlankPositions = [0, 1, 2],
                Solutions = [a, b, c],
                AllowsExponents = false
            };

            return (
                JsonSerializer.Serialize(puzzle),
                $"{a}, {b}, {c}",
                "Remember PEMDAS: Do B ÷ C first, then subtract from A. All three values must be different integers. Negative values are allowed!",
                400,
                0
            );
        }
    }

    private (string puzzleData, string solution, string hint, int points, int timeLimit) GenerateBossSolveIt(Random random)
    {
        // Generate PEMDAS-focused Boss puzzle with exponents
        var puzzleType = random.Next(0, 2);
        
        if (puzzleType == 0)
        {
            // Type A: Exponent Priority (e.g., X² + Y × 3 = result)
            var x = random.Next(2, 5);
            var y = random.Next(2, 8);
            
            // Ensure X and Y are different
            while (x == y)
            {
                y = random.Next(2, 8);
            }
            
            var multiplier = random.Next(2, 4);
            var result = (x * x) + (y * multiplier);

            var puzzle = new SolveItPuzzle
            {
                Equation = $"X² + Y × {multiplier} = {result}",
                BlankPositions = [0, 1],
                Solutions = [x, y],
                AllowsExponents = true
            };

            return (
                JsonSerializer.Serialize(puzzle),
                $"{x}, {y}",
                $"Remember PEMDAS: First calculate X² (exponent), then Y × {multiplier} (multiply), finally add them. X and Y must be different. Negative values are allowed!",
                500,
                0
            );
        }
        else
        {
            // Type B: Full PEMDAS Chain (e.g., X² × Y - Z = result)
            var x = random.Next(2, 4);
            var y = random.Next(2, 4);
            var z = random.Next(2, 10);
            
            // Ensure all different
            while (x == y)
            {
                y = random.Next(2, 4);
            }
            while (z == x || z == y)
            {
                z = random.Next(2, 10);
            }
            
            var result = (x * x * y) - z;
            
            // Ensure positive result
            if (result <= 0)
            {
                z = random.Next(1, (x * x * y) - 1);
                result = (x * x * y) - z;
            }

            var puzzle = new SolveItPuzzle
            {
                Equation = $"X² × Y - Z = {result}",
                BlankPositions = [0, 1, 2],
                Solutions = [x, y, z],
                AllowsExponents = true
            };

            return (
                JsonSerializer.Serialize(puzzle),
                $"{x}, {y}, {z}",
                "PEMDAS chain: X² (exponent first), then × Y (multiply), finally - Z (subtract). All values must be different. Negative values are allowed!",
                500,
                0
            );
        }
    }

    private (string puzzleData, string solution, string hint, int points, int timeLimit) GenerateExpertSolveIt(Random random)
    {
        // Expert level: Advanced mathematical concepts with integer answers only
        var puzzleType = random.Next(0, 4);
        
        if (puzzleType == 0)
        {
            // Type A: Simple Exponential (e.g., 2^? = 16)
            var bases = new[] { 2, 3 };
            var baseNum = bases[random.Next(bases.Length)];
            var exponent = random.Next(2, 5); // Keep reasonable: 2, 3, or 4
            var result = (int)Math.Pow(baseNum, exponent);

            var puzzle = new SolveItPuzzle
            {
                Equation = $"{baseNum}^? = {result}",
                BlankPositions = [0],
                Solutions = [exponent],
                AllowsExponents = true
            };

            return (
                JsonSerializer.Serialize(puzzle),
                exponent.ToString(),
                $"What power of {baseNum} equals {result}? Think: {baseNum} × {baseNum} × ...",
                600,
                0
            );
        }
        else if (puzzleType == 1)
        {
            // Type B: Exponential with expression (e.g., 2^(3-?) = 1/4, so 2^(3-?) = 2^-2, thus ? = 5)
            // But user enters integer result: 2^(3-?) = 4, means 2^2 = 4, so 3-? = 2, ? = 1
            var baseNum = 2;
            var targetExp = random.Next(1, 4); // 1, 2, or 3
            var constantInExp = targetExp + random.Next(1, 4); // Make sure constant > targetExp
            var answer = constantInExp - targetExp;
            var result = (int)Math.Pow(baseNum, targetExp);

            var puzzle = new SolveItPuzzle
            {
                Equation = $"{baseNum}^({constantInExp}-?) = {result}",
                BlankPositions = [0],
                Solutions = [answer],
                AllowsExponents = true
            };

            return (
                JsonSerializer.Serialize(puzzle),
                answer.ToString(),
                $"Solve the exponent: {baseNum}^? = {result}, so {constantInExp}-? must equal that exponent.",
                600,
                0
            );
        }
        else if (puzzleType == 2)
        {
            // Type C: Logarithm concept (e.g., log?(?) = 3, means 2^3 = ?, so ? = 8)
            var bases = new[] { 2, 3 };
            var baseNum = bases[random.Next(bases.Length)];
            var exponent = random.Next(2, 4); // 2 or 3
            var answer = (int)Math.Pow(baseNum, exponent);

            var puzzle = new SolveItPuzzle
            {
                Equation = $"log?{baseNum}?(?) = {exponent}",
                BlankPositions = [0],
                Solutions = [answer],
                AllowsExponents = true
            };

            return (
                JsonSerializer.Serialize(puzzle),
                answer.ToString(),
                $"log?{baseNum}?(?) = {exponent} means {baseNum}^{exponent} = ?. What is {baseNum} to the power of {exponent}?",
                600,
                0
            );
        }
        else
        {
            // Type D: Basic calculus - derivative constant
            // d/dx(ax² + b) = 2ax, find the constant 'a'
            // We show: d/dx(?x²) = 6x, so 2×? = 6, thus ? = 3
            var coefficient = random.Next(2, 6);
            var derivativeCoef = 2 * coefficient;

            var puzzle = new SolveItPuzzle
            {
                Equation = $"d/dx(?x²) = {derivativeCoef}x",
                BlankPositions = [0],
                Solutions = [coefficient],
                AllowsExponents = true
            };

            return (
                JsonSerializer.Serialize(puzzle),
                coefficient.ToString(),
                $"Power rule: bring down the exponent and multiply. If d/dx(?x²) = {derivativeCoef}x, then 2×? = {derivativeCoef}.",
                600,
                0
            );
        }
    }

    private (string puzzleData, string solution, string hint, int points, int timeLimit) GenerateBuildItPuzzle(DifficultyLevel difficulty, int seed)
    {
        try
        {
            var random = new Random(seed);

            return difficulty switch
            {
                DifficultyLevel.Medium => GenerateMediumBuildIt(random),
                DifficultyLevel.Creative => GenerateCreativeBuildIt(random),
                DifficultyLevel.Speed => GenerateSpeedBuildIt(random),
                _ => GenerateMediumBuildIt(random)
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error generating Build It puzzle: {ex.Message}");
            return GenerateMediumBuildIt(new Random(seed));
        }
    }

    private (string puzzleData, string solution, string hint, int points, int timeLimit) GenerateMediumBuildIt(Random random)
    {
        // Alternate between regular and "No Parentheses" challenges
        var isPEMDASChallenge = random.Next(0, 2) == 0;
        
        if (isPEMDASChallenge)
        {
            // PEMDAS Challenge: No parentheses allowed - ALL SOLUTIONS VERIFIED
            var targetOptions = new[]
            {
                // Target 19: 5*4-3+2 = 20-3+2 = 19 ? | 4*5-3+2 = 20-3+2 = 19 ?
                (target: 19, digits: new[] { 2, 3, 4, 5 }, solutions: new[] { "5 * 4 - 3 + 2", "4 * 5 - 3 + 2" }),
                // Target 14: 6*3-5+1 = 18-5+1 = 14 ? | 3*6-5+1 = 18-5+1 = 14 ?
                (target: 14, digits: new[] { 1, 3, 5, 6 }, solutions: new[] { "6 * 3 - 5 + 1", "3 * 6 - 5 + 1" }),
                // Target 23: 7*3+4-2 = 21+4-2 = 23 ? | 3*7+4-2 = 21+4-2 = 23 ?
                (target: 23, digits: new[] { 2, 3, 4, 7 }, solutions: new[] { "7 * 3 + 4 - 2", "3 * 7 + 4 - 2" })
            };
            
            var selected = targetOptions[random.Next(targetOptions.Length)];
            
            var puzzle = new BuildItPuzzle
            {
                AvailableDigits = [.. selected.digits],
                TargetNumber = selected.target,
                AcceptedSolutions = [.. selected.solutions],
                MaxParentheses = 0,
                AllowsAdvancedOperators = false
            };

            return (
                JsonSerializer.Serialize(puzzle),
                selected.solutions[0],
                "PEMDAS Challenge! No parentheses allowed. Remember: Multiply before adding or subtracting.",
                200,
                0
            );
        }
        else
        {
            // Regular Build It with limited parentheses - ALL SOLUTIONS VERIFIED
            var targetOptions = new[]
            {
                // Target 24: All 3 solutions verified ?
                (target: 24, digits: new[] { 2, 3, 4, 6 }, solutions: new[] { "3 * 6 + 4 + 2", "(2 + 4) * 3 + 6", "(6 / 2 + 3) * 4" }),
                // Target 29: Only 2 verified solutions
                (target: 29, digits: new[] { 2, 3, 5, 6 }, solutions: new[] { "6 * 5 - 3 + 2", "5 * 6 - 3 + 2" }),
                // Target 18: Only 2 verified solutions
                (target: 18, digits: new[] { 1, 3, 4, 7 }, solutions: new[] { "7 * 3 - 4 + 1", "3 * 7 - 4 + 1" })
            };
            
            var selected = targetOptions[random.Next(targetOptions.Length)];
            
            var puzzle = new BuildItPuzzle
            {
                AvailableDigits = [.. selected.digits],
                TargetNumber = selected.target,
                AcceptedSolutions = [.. selected.solutions],
                MaxParentheses = 1,
                AllowsAdvancedOperators = false
            };

            return (
                JsonSerializer.Serialize(puzzle),
                selected.solutions[0],
                "One set of parentheses allowed. Bonus points for solutions without parentheses!",
                200,
                0
            );
        }
    }

    private (string puzzleData, string solution, string hint, int points, int timeLimit) GenerateCreativeBuildIt(Random random)
    {
        // Creative mode encourages elegant solutions - ALL SOLUTIONS MANUALLY VERIFIED
        var targetVariations = new[]
        {
            // 6*(9-3)-2 = 6*6-2 = 34 ? | (9-3)*6-2 = 6*6-2 = 34 ?
            (target: 34, digits: new[] { 2, 3, 6, 9 }, solutions: new[] { "6 * (9 - 3) - 2", "(9 - 3) * 6 - 2" }),
            // 7*6-5+3 = 42-5+3 = 40 ? | 6*7-5+3 = 42-5+3 = 40 ?
            (target: 40, digits: new[] { 3, 5, 6, 7 }, solutions: new[] { "7 * 6 - 5 + 3", "6 * 7 - 5 + 3" }),
            // 8*6-4+2 = 48-4+2 = 46 ? | 6*8-4+2 = 48-4+2 = 46 ?
            (target: 46, digits: new[] { 2, 4, 6, 8 }, solutions: new[] { "8 * 6 - 4 + 2", "6 * 8 - 4 + 2" }),
            // 8*3-5+2 = 24-5+2 = 21 ? | 3*8-5+2 = 24-5+2 = 21 ?
            (target: 21, digits: new[] { 2, 3, 5, 8 }, solutions: new[] { "8 * 3 - 5 + 2", "3 * 8 - 5 + 2" }),
            // 8*5-4+2 = 40-4+2 = 38 ? | 5*8-4+2 = 40-4+2 = 38 ?
            (target: 38, digits: new[] { 2, 4, 5, 8 }, solutions: new[] { "8 * 5 - 4 + 2", "5 * 8 - 4 + 2" })
        };
        
        var selected = targetVariations[random.Next(targetVariations.Length)];
        
        var puzzle = new BuildItPuzzle
        {
            AvailableDigits = [.. selected.digits],
            TargetNumber = selected.target,
            AcceptedSolutions = [.. selected.solutions],
            MaxParentheses = 2,
            AllowsAdvancedOperators = false
        };

        return (
            JsonSerializer.Serialize(puzzle),
            selected.solutions[0],
            "Elegant solutions (fewest symbols) earn extra points. Try solving without parentheses if possible!",
            350,
            0
        );
    }

    private (string puzzleData, string solution, string hint, int points, int timeLimit) GenerateSpeedBuildIt(Random random)
    {
        var puzzle = new BuildItPuzzle
        {
            AvailableDigits = Enumerable.Range(1, 9).ToList(),
            TargetNumber = random.Next(30, 60),
            AcceptedSolutions = ["Multiple solutions possible"],
            MaxParentheses = 3,
            AllowsAdvancedOperators = false
        };

        return (
            JsonSerializer.Serialize(puzzle),
            "Multiple solutions",
            "Build equation in under 60 seconds. Multiple solutions accepted!",
            500,
            60
        );
    }

    public async Task ClearAndRegeneratePuzzles()
    {
        try
        {
            await Init();
            
            System.Diagnostics.Debug.WriteLine("?? === Starting Puzzle Regeneration ===");
            System.Diagnostics.Debug.WriteLine("   Deleting all existing puzzles...");
            
            // Delete all existing puzzles
            await _database!.ExecuteAsync("DELETE FROM DailyPuzzles");
            
            // Clear cache
            _cachedTodaysPuzzle = null;
            
            System.Diagnostics.Debug.WriteLine("   Generating new puzzles with proper difficulty slots...");
            
            // Regenerate puzzles with correct difficulty slot assignments
            await InitializePuzzles();
            
            // Verify generation
            var count = await _database.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM DailyPuzzles");
            var today = DateTime.UtcNow.Date;
            var todayCount = await _database.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM DailyPuzzles WHERE PuzzleDate = ?", today);
            
            System.Diagnostics.Debug.WriteLine("? === Puzzle Regeneration Complete ===");
            System.Diagnostics.Debug.WriteLine($"   Total puzzles: {count} (Expected: 43,800)");
            System.Diagnostics.Debug.WriteLine($"   Today's puzzles: {todayCount} (Expected: 8)");
            
            if (todayCount == 8)
            {
                // Show today's puzzle assignments
                var todayPuzzles = await _database.QueryAsync<DailyPuzzle>(
                    "SELECT DifficultySlot, Difficulty, Mode FROM DailyPuzzles WHERE PuzzleDate = ? ORDER BY DifficultySlot",
                    today);
                
                System.Diagnostics.Debug.WriteLine("   Today's slot assignments:");
                foreach (var p in todayPuzzles)
                {
                    System.Diagnostics.Debug.WriteLine($"      Slot {p.DifficultySlot}: {p.Difficulty} - {p.Mode}");
                }
                
                // Verify slot 0 is Easy
                var slot0 = todayPuzzles.FirstOrDefault();
                if (slot0 != null && slot0.Difficulty == DifficultyLevel.Easy)
                {
                    System.Diagnostics.Debug.WriteLine("? Verification passed: Slot 0 = Easy");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"? Verification FAILED: Slot 0 = {slot0?.Difficulty}");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"? Error regenerating puzzles: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
            throw;
        }
    }

    // Check if puzzles need regeneration
    public async Task<bool> NeedsRegeneration()
    {
        try
        {
            await Init();
            
            // Check if any puzzles exist
            var count = await _database!.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM DailyPuzzles");
            if (count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No puzzles found - regeneration needed");
                return true;
            }
            
            // Get today's puzzle to check format
            var today = DateTime.UtcNow.Date;
            var puzzle = await _database!.Table<DailyPuzzle>()
                .Where(p => p.PuzzleDate == today)
                .FirstOrDefaultAsync();
            
            if (puzzle != null)
            {
                // Check for old notation
                if (puzzle.PuzzleData.Contains("_"))
                {
                    System.Diagnostics.Debug.WriteLine("Old underscore notation detected - regeneration needed");
                    return true;
                }
                
                // Check for old PEMDAS format
                if (puzzle.Mode == PuzzleMode.SolveIt && puzzle.Difficulty == DifficultyLevel.Easy)
                {
                    var solveItPuzzle = JsonSerializer.Deserialize<SolveItPuzzle>(puzzle.PuzzleData);
                    if (solveItPuzzle != null && solveItPuzzle.Equation.StartsWith("(? +"))
                    {
                        System.Diagnostics.Debug.WriteLine("Old Easy format with parentheses detected - regeneration needed");
                        return true;
                    }
                }
            }
            
            System.Diagnostics.Debug.WriteLine("Puzzles are up to date - no regeneration needed");
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking puzzle status: {ex.Message}");
            return true; // Regenerate on error to be safe
        }
    }

    // Performance: Cleanup method to clear caches
    public void ClearCache()
    {
        _cachedUserProgress = null;
        _cachedTodaysPuzzle = null;
    }
    
    // Daily Completion Tracking Methods
    public async Task<bool> HasCompletedDifficultyToday(int difficultySlot)
    {
        try
        {
            await Init();
            var today = DateTime.UtcNow.Date;
            var dateSlotKey = $"{today:yyyy-MM-dd}_{difficultySlot}";
            
            var completion = await _database!.Table<DailyCompletion>()
                .Where(c => c.DateSlotKey == dateSlotKey)
                .FirstOrDefaultAsync();
                
            return completion != null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking completion: {ex.Message}");
            return false;
        }
    }
    
    public async Task<List<int>> GetTodaysCompletedDifficulties()
    {
        try
        {
            await Init();
            var today = DateTime.UtcNow.Date;
            
            var completions = await _database!.Table<DailyCompletion>()
                .Where(c => c.CompletionDate == today)
                .ToListAsync();
                
            return completions.Select(c => c.DifficultySlot).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting completed difficulties: {ex.Message}");
            return new List<int>();
        }
    }
    
    public async Task<bool> RecordDailyCompletion(int difficultySlot, string puzzleId, int completionTime, int pointsEarned, bool adWatched)
    {
        try
        {
            await Init();
            var today = DateTime.UtcNow.Date;
            var dateSlotKey = $"{today:yyyy-MM-dd}_{difficultySlot}";
            
            // Check if already completed
            var existing = await _database!.Table<DailyCompletion>()
                .Where(c => c.DateSlotKey == dateSlotKey)
                .FirstOrDefaultAsync();
                
            if (existing != null)
            {
                System.Diagnostics.Debug.WriteLine($"Difficulty {difficultySlot} already completed today");
                return false;
            }
            
            var completion = new DailyCompletion
            {
                CompletionDate = today,
                DifficultySlot = difficultySlot,
                DateSlotKey = dateSlotKey,
                PuzzleIdentifier = puzzleId,
                CompletionTime = completionTime,
                PointsEarned = pointsEarned,
                AdWatched = adWatched
            };
            
            await _database!.InsertAsync(completion);
            System.Diagnostics.Debug.WriteLine($"? Recorded completion for difficulty {difficultySlot}: {puzzleId}");
            
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error recording completion: {ex.Message}");
            return false;
        }
    }
    
    public async Task<DailyCompletion?> GetCompletionStats(int difficultySlot, DateTime date)
    {
        try
        {
            await Init();
            var dateSlotKey = $"{date:yyyy-MM-dd}_{difficultySlot}";
            
            return await _database!.Table<DailyCompletion>()
                .Where(c => c.DateSlotKey == dateSlotKey)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting completion stats: {ex.Message}");
            return null;
        }
    }
    
    public async Task<int> GetTotalPointsEarned()
    {
        try
        {
            await Init();
            
            // Sum all points from completions
            var completions = await _database!.Table<DailyCompletion>().ToListAsync();
            return completions.Sum(c => c.PointsEarned);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting total points: {ex.Message}");
            return 0;
        }
    }
    
    public async Task<List<DailyCompletion>> GetRecentCompletions(int days = 7)
    {
        try
        {
            await Init();
            var cutoffDate = DateTime.UtcNow.Date.AddDays(-days);
            
            var completions = await _database!.Table<DailyCompletion>()
                .Where(c => c.CompletionDate >= cutoffDate)
                .OrderByDescending(c => c.CompletionDate)
                .ThenBy(c => c.CompletionTime)
                .ToListAsync();
                
            return completions ?? new List<DailyCompletion>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting recent completions: {ex.Message}");
            return new List<DailyCompletion>();
        }
    }
}
