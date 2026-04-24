using SQLite;

namespace BadlyDefined.Models;

/// <summary>
/// Represents a daily bad definition puzzle
/// </summary>
[Table("BadDefinitions")]
public class BadDefinition
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Date for this puzzle (format: YYYY-MM-DD)
    /// </summary>
    public DateTime PuzzleDate { get; set; }

    /// <summary>
    /// Unique identifier for this puzzle (format: BD20250205-E for Easy, M for Medium, H for Hard)
    /// </summary>
    public string PuzzleIdentifier { get; set; } = string.Empty;

    /// <summary>
    /// Difficulty slot: 0 = Easy, 1 = Medium, 2 = Hard
    /// </summary>
    public int DifficultySlot { get; set; }

    /// <summary>
    /// Difficulty level enum value
    /// </summary>
    public DifficultyLevel Difficulty { get; set; }

    /// <summary>
    /// The actual answer/solution word or phrase
    /// </summary>
    public string Solution { get; set; } = string.Empty;

    /// <summary>
    /// The bad/funny/misleading definition
    /// </summary>
    public string BadDefinitionText { get; set; } = string.Empty;

    /// <summary>
    /// Category for first-level hint (Person, Place, Thing, Animal, Plant, Job, etc.)
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Number of letters in the solution (including spaces for phrases)
    /// </summary>
    public int LetterCount { get; set; }

    /// <summary>
    /// Base points for solving this puzzle (before penalties)
    /// </summary>
    public int BasePoints { get; set; }

    /// <summary>
    /// Indices of letters to reveal on first wrong guess (comma-separated, e.g., "0,4,8")
    /// </summary>
    public string FirstHintLetters { get; set; } = string.Empty;

    /// <summary>
    /// Indices of letters to reveal on second wrong guess
    /// </summary>
    public string SecondHintLetters { get; set; } = string.Empty;

    /// <summary>
    /// Indices of letters to reveal on third wrong guess
    /// </summary>
    public string ThirdHintLetters { get; set; } = string.Empty;

    /// <summary>
    /// Maximum number of wrong guesses before revealing solution
    /// </summary>
    public int MaxAttempts { get; set; } = 5;
}

/// <summary>
/// Difficulty levels for Bad Definition puzzles
/// </summary>
public enum DifficultyLevel
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}
