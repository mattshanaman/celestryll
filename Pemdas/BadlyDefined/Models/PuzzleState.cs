namespace BadlyDefined.Models;

/// <summary>
/// Tracks a single guess attempt for the current puzzle
/// </summary>
public class GuessAttempt
{
    /// <summary>
    /// The guess text entered by user
    /// </summary>
    public string GuessText { get; set; } = string.Empty;

    /// <summary>
    /// Whether this guess was correct
    /// </summary>
    public bool IsCorrect { get; set; }

    /// <summary>
    /// Attempt number (1-based)
    /// </summary>
    public int AttemptNumber { get; set; }

    /// <summary>
    /// Timestamp of the attempt
    /// </summary>
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Represents the current state of a puzzle being played
/// </summary>
public class PuzzleState
{
    /// <summary>
    /// The puzzle being played
    /// </summary>
    public BadDefinition? CurrentPuzzle { get; set; }

    /// <summary>
    /// Number of attempts made so far
    /// </summary>
    public int AttemptsCount { get; set; }

    /// <summary>
    /// List of all guess attempts
    /// </summary>
    public List<GuessAttempt> Attempts { get; set; } = new();

    /// <summary>
    /// Current hint level (0 = no hints, 1 = category, 2 = first letters, 3 = more letters, etc.)
    /// </summary>
    public int CurrentHintLevel { get; set; }

    /// <summary>
    /// Letters revealed so far (indices)
    /// </summary>
    public HashSet<int> RevealedLetters { get; set; } = new();

    /// <summary>
    /// Whether puzzle is completed
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Time when puzzle started
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Time when puzzle completed
    /// </summary>
    public DateTime? CompletionTime { get; set; }

    /// <summary>
    /// Points that will be earned (decreases with wrong guesses)
    /// </summary>
    public int CurrentPoints { get; set; }
}
