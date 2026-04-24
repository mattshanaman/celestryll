using SQLite;

namespace BadlyDefined.Models;

/// <summary>
/// Tracks user's overall progress and statistics
/// </summary>
[Table("UserProgress")]
public class UserProgress
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Current streak of consecutive days with at least one puzzle completed
    /// </summary>
    public int CurrentStreak { get; set; }

    /// <summary>
    /// Longest streak ever achieved
    /// </summary>
    public int LongestStreak { get; set; }

    /// <summary>
    /// Total puzzles completed across all difficulties
    /// </summary>
    public int TotalPuzzlesCompleted { get; set; }

    /// <summary>
    /// Total points earned
    /// </summary>
    public int TotalPoints { get; set; }

    /// <summary>
    /// Number of hint tokens available (earned or purchased)
    /// </summary>
    public int HintTokens { get; set; }

    /// <summary>
    /// Last date user played (for streak calculation)
    /// </summary>
    public DateTime? LastPlayDate { get; set; }

    /// <summary>
    /// Preferred difficulty slot (0-2)
    /// </summary>
    public int PreferredDifficultySlot { get; set; }

    /// <summary>
    /// Whether user has watched ad today (for difficulty unlock)
    /// </summary>
    public bool HasWatchedAdToday { get; set; }

    /// <summary>
    /// Last date ad was watched
    /// </summary>
    public DateTime? LastAdWatchDate { get; set; }

    /// <summary>
    /// Total number of Easy puzzles completed
    /// </summary>
    public int EasyCompleted { get; set; }

    /// <summary>
    /// Total number of Medium puzzles completed
    /// </summary>
    public int MediumCompleted { get; set; }

    /// <summary>
    /// Total number of Hard puzzles completed
    /// </summary>
    public int HardCompleted { get; set; }

    /// <summary>
    /// Average attempts per puzzle
    /// </summary>
    public double AverageAttempts { get; set; }

    /// <summary>
    /// Best (lowest) attempts to solve a puzzle
    /// </summary>
    public int BestAttempts { get; set; } = int.MaxValue;

    /// <summary>
    /// User's email address for notifications and account recovery
    /// </summary>
    [MaxLength(255)]
    public string? Email { get; set; }

    /// <summary>
    /// Date when user profile was created
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last time user progress was updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
