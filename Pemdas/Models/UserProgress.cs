using SQLite;

namespace Pemdas.Models
{
    [SQLite.Table("UserProgress")]
    public class UserProgress
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int HintTokens { get; set; }

        [Indexed]
        public DateTime LastPlayedDate { get; set; }

        public int TotalPuzzlesSolved { get; set; }
        public int TotalPoints { get; set; }
        public bool IsSubscribed { get; set; }
        
        // Premium features
        public DateTime? SubscriptionExpiry { get; set; }
        public DateTime? LastAdWatchDate { get; set; }
        public bool HasWatchedAdToday { get; set; }
        
        // User's preferred difficulty (0-7 for Easy through Expert)
        public int PreferredDifficultySlot { get; set; } = 0; // Default to Easy

        // User email for stats sharing and notifications
        [MaxLength(255)]
        public string? Email { get; set; }

        // Profile creation and update tracking
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    [SQLite.Table("PuzzleAttempts")]
    public class PuzzleAttempt
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int PuzzleId { get; set; }

        [Indexed]
        public DateTime AttemptDate { get; set; }

        public bool Solved { get; set; }
        
        // Time spent in seconds
        public int TimeSpent { get; set; }
        public int PointsEarned { get; set; }

        [SQLite.MaxLength(500)]
        public string UserSolution { get; set; } = string.Empty;

        public int AttemptsCount { get; set; }
        
        // Track which difficulty was completed (for daily completion)
        public int DifficultySlot { get; set; }
        
        // Puzzle identifier for sharing
        [SQLite.MaxLength(50)]
        public string PuzzleIdentifier { get; set; } = string.Empty;
    }
    
    // Track which difficulties have been completed each day
    [SQLite.Table("DailyCompletions")]
    public class DailyCompletion
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        [Indexed]
        public DateTime CompletionDate { get; set; }
        
        // Which difficulty slot (0-7)
        public int DifficultySlot { get; set; }
        
        // Whether an ad was watched for this completion
        public bool AdWatched { get; set; }
        
        // Completion time in seconds
        public int CompletionTime { get; set; }
        
        // Points earned
        public int PointsEarned { get; set; }
        
        // Puzzle identifier
        [SQLite.MaxLength(50)]
        public string PuzzleIdentifier { get; set; } = string.Empty;
        
        // Create a composite index for efficient lookup
        [Indexed]
        public string DateSlotKey { get; set; } = string.Empty; // Format: "2024-01-15_3"
    }
}