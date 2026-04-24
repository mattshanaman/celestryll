using SQLite;

namespace BadlyDefined.Models;

/// <summary>
/// Tracks completion of individual puzzles (supports multiple completions per day for different difficulties)
/// </summary>
[Table("DailyCompletions")]
public class DailyCompletion
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Date when puzzle was completed
    /// </summary>
    public DateTime CompletionDate { get; set; }

    /// <summary>
    /// Difficulty slot (0-2)
    /// </summary>
    public int DifficultySlot { get; set; }

    /// <summary>
    /// Unique puzzle identifier (e.g., BD20250205-E)
    /// </summary>
    public string PuzzleIdentifier { get; set; } = string.Empty;

    /// <summary>
    /// Number of attempts taken to solve
    /// </summary>
    public int AttemptsCount { get; set; }

    /// <summary>
    /// Time taken to complete (in seconds)
    /// </summary>
    public int CompletionTime { get; set; }

    /// <summary>
    /// Points earned for this completion
    /// </summary>
    public int PointsEarned { get; set; }

    /// <summary>
    /// Whether user watched an ad for this puzzle
    /// </summary>
    public bool AdWatched { get; set; }

    /// <summary>
    /// Composite key for quick lookups (format: "2025-02-05_0" for date + difficulty slot)
    /// </summary>
    [Indexed(Name = "IX_DateSlot", Unique = true)]
    public string DateSlotKey { get; set; } = string.Empty;
}
