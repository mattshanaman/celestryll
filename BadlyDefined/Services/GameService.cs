using BadlyDefined.Models;
using System.Diagnostics;
using System.Text;

namespace BadlyDefined.Services;

/// <summary>
/// Handles game logic, scoring, and puzzle state for BadlyDefined
/// </summary>
public class GameService
{
    private readonly DatabaseService _databaseService;
    private PuzzleState _currentState;
    private int _currentDifficultySlot;

    public GameService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        _currentState = new PuzzleState();
        _currentDifficultySlot = 0;
    }

    // ==================== PUZZLE MANAGEMENT ====================

    public async Task<(BadDefinition? puzzle, bool alreadyPlayed)> GetTodaysPuzzleAsync(int difficultySlot)
    {
        try
        {
            _currentDifficultySlot = difficultySlot;
            var puzzle = await _databaseService.GetTodaysPuzzleAsync(difficultySlot);
            
            if (puzzle == null)
            {
                Debug.WriteLine($"⚠️ No puzzle found for slot {difficultySlot}");
                return (null, false);
            }

            // Check if already completed
            var alreadyPlayed = await _databaseService.HasCompletedDifficultyTodayAsync(difficultySlot);

            Debug.WriteLine($"📋 Puzzle loaded: {puzzle.Solution} (Already played: {alreadyPlayed})");
            return (puzzle, alreadyPlayed);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error getting puzzle: {ex.Message}");
            return (null, false);
        }
    }

    public void StartPuzzle(BadDefinition puzzle)
    {
        _currentState = new PuzzleState
        {
            CurrentPuzzle = puzzle,
            AttemptsCount = 0,
            Attempts = new List<GuessAttempt>(),
            CurrentHintLevel = 0,
            RevealedLetters = new HashSet<int>(),
            IsCompleted = false,
            StartTime = DateTime.UtcNow,
            CurrentPoints = puzzle.BasePoints
        };

        Debug.WriteLine($"🎮 Puzzle started: {puzzle.Solution}");
    }

    public PuzzleState GetCurrentState() => _currentState;

    // ==================== GUESS HANDLING ====================

    public GuessResult SubmitGuess(string guess)
    {
        if (_currentState.CurrentPuzzle == null)
        {
            return new GuessResult
            {
                IsCorrect = false,
                Message = "No active puzzle!",
                PointsEarned = 0
            };
        }

        // Normalize guess and solution for comparison
        var normalizedGuess = NormalizeText(guess);
        var normalizedSolution = NormalizeText(_currentState.CurrentPuzzle.Solution);

        _currentState.AttemptsCount++;

        var attempt = new GuessAttempt
        {
            GuessText = guess,
            IsCorrect = normalizedGuess == normalizedSolution,
            AttemptNumber = _currentState.AttemptsCount,
            Timestamp = DateTime.UtcNow
        };

        _currentState.Attempts.Add(attempt);

        if (attempt.IsCorrect)
        {
            // Correct guess!
            _currentState.IsCompleted = true;
            _currentState.CompletionTime = DateTime.UtcNow;

            var points = CalculatePoints(
                _currentState.AttemptsCount,
                _currentState.CurrentPuzzle.BasePoints
            );

            Debug.WriteLine($"✅ Correct guess! {points} points earned");

            return new GuessResult
            {
                IsCorrect = true,
                Message = "Correct! 🎉",
                PointsEarned = points,
                AttemptsUsed = _currentState.AttemptsCount
            };
        }
        else
        {
            // Wrong guess - reveal more letters
            RevealNextHint();

            // Reduce points
            var penaltyPerGuess = _currentState.CurrentPuzzle.BasePoints / 5;
            _currentState.CurrentPoints = Math.Max(
                10, // Minimum points
                _currentState.CurrentPoints - penaltyPerGuess
            );

            Debug.WriteLine($"❌ Wrong guess. Attempts: {_currentState.AttemptsCount}, Points remaining: {_currentState.CurrentPoints}");

            return new GuessResult
            {
                IsCorrect = false,
                Message = "Not quite right, try again!",
                PointsEarned = 0,
                AttemptsUsed = _currentState.AttemptsCount,
                HintRevealed = true,
                RevealedWord = GetRevealedWord()
            };
        }
    }

    public void RevealNextHint()
    {
        if (_currentState.CurrentPuzzle == null)
            return;

        _currentState.CurrentHintLevel++;

        // Reveal letters based on hint level
        var lettersToReveal = _currentState.CurrentHintLevel switch
        {
            1 => _currentState.CurrentPuzzle.FirstHintLetters,
            2 => _currentState.CurrentPuzzle.SecondHintLetters,
            3 => _currentState.CurrentPuzzle.ThirdHintLetters,
            _ => "" // Reveal all remaining letters
        };

        if (!string.IsNullOrEmpty(lettersToReveal))
        {
            var indices = lettersToReveal.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse);
            
            foreach (var index in indices)
            {
                _currentState.RevealedLetters.Add(index);
            }
        }
        else if (_currentState.CurrentHintLevel > 3)
        {
            // Reveal all remaining letters progressively
            var solution = _currentState.CurrentPuzzle.Solution;
            var unrevealed = Enumerable.Range(0, solution.Length)
                .Where(i => !_currentState.RevealedLetters.Contains(i) && solution[i] != ' ')
                .ToList();

            if (unrevealed.Any())
            {
                // Reveal 1-2 more letters
                var count = Math.Min(2, unrevealed.Count);
                for (int i = 0; i < count; i++)
                {
                    _currentState.RevealedLetters.Add(unrevealed[i]);
                }
            }
        }

        Debug.WriteLine($"💡 Hint level {_currentState.CurrentHintLevel} revealed");
    }

    public string GetHintText()
    {
        if (_currentState.CurrentPuzzle == null)
            return "";

        return _currentState.CurrentHintLevel switch
        {
            0 => $"Category: {_currentState.CurrentPuzzle.Category}",
            1 => $"Category: {_currentState.CurrentPuzzle.Category}\n" +
                 $"First letters revealed!",
            2 => $"More letters revealed!",
            3 => $"Even more letters revealed!",
            _ => "Almost there! More letters revealed!"
        };
    }

    public string GetRevealedWord()
    {
        if (_currentState.CurrentPuzzle == null)
            return "";

        var solution = _currentState.CurrentPuzzle.Solution;
        var sb = new StringBuilder();

        for (int i = 0; i < solution.Length; i++)
        {
            if (solution[i] == ' ')
            {
                sb.Append("  "); // Space
            }
            else if (solution[i] == '-')
            {
                sb.Append("- "); // Hyphen (always show)
            }
            else if (_currentState.RevealedLetters.Contains(i))
            {
                sb.Append($"{solution[i]} "); // Revealed letter
            }
            else
            {
                sb.Append("_ "); // Hidden letter
            }
        }

        return sb.ToString().Trim();
    }

    // ==================== SCORING ====================

    public int CalculatePoints(int attempts, int basePoints)
    {
        // Penalty per wrong guess
        var penaltyPerGuess = basePoints / 5;
        var totalPenalty = (attempts - 1) * penaltyPerGuess; // -1 because first guess has no penalty
        
        var points = basePoints - totalPenalty;
        
        // Minimum 10 points
        return Math.Max(10, points);
    }

    public async Task<bool> CompletePuzzleAsync()
    {
        if (_currentState.CurrentPuzzle == null || !_currentState.IsCompleted)
            return false;

        try
        {
            var completionTime = _currentState.CompletionTime.HasValue
                ? (int)(_currentState.CompletionTime.Value - _currentState.StartTime).TotalSeconds
                : 0;

            var completion = new DailyCompletion
            {
                CompletionDate = DateTime.UtcNow.Date,
                DifficultySlot = _currentDifficultySlot,
                PuzzleIdentifier = _currentState.CurrentPuzzle.PuzzleIdentifier,
                AttemptsCount = _currentState.AttemptsCount,
                CompletionTime = completionTime,
                PointsEarned = _currentState.CurrentPoints,
                AdWatched = false // Will be set by ViewModel
            };

            var recorded = await _databaseService.RecordCompletionAsync(completion);
            
            if (recorded)
            {
                Debug.WriteLine($"✅ Puzzle completion recorded: {_currentState.CurrentPoints} points");
            }

            return recorded;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error completing puzzle: {ex.Message}");
            return false;
        }
    }

    // ==================== SHARING ====================

    public string GenerateShareableResult(BadDefinition puzzle, int attempts, int points, int completionTime)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"🎯 BadlyDefined {puzzle.PuzzleIdentifier}");
        sb.AppendLine($"Difficulty: {GetDifficultyName(puzzle.DifficultySlot)}");
        sb.AppendLine($"Attempts: {attempts}");
        sb.AppendLine($"Time: {FormatTime(completionTime)}");
        sb.AppendLine($"Points: {points}");
        sb.AppendLine();
        sb.AppendLine($"\"{puzzle.BadDefinitionText}\"");
        sb.AppendLine($"Answer: {puzzle.Solution}");
        sb.AppendLine();
        sb.AppendLine("Can you beat my score? Download BadlyDefined now!");

        return sb.ToString();
    }

    // ==================== HELPERS ====================

    private string NormalizeText(string text)
    {
        return text.Trim()
            .Replace(" ", "")
            .Replace("-", "")
            .ToUpperInvariant();
    }

    private string GetDifficultyName(int slot) => slot switch
    {
        0 => "Easy ⭐",
        1 => "Medium ⭐⭐",
        2 => "Hard ⭐⭐⭐",
        _ => "Unknown"
    };

    private string FormatTime(int seconds)
    {
        var minutes = seconds / 60;
        var secs = seconds % 60;
        return $"{minutes}:{secs:D2}";
    }
}

/// <summary>
/// Result of a guess attempt
/// </summary>
public class GuessResult
{
    public bool IsCorrect { get; set; }
    public string Message { get; set; } = "";
    public int PointsEarned { get; set; }
    public int AttemptsUsed { get; set; }
    public bool HintRevealed { get; set; }
    public string RevealedWord { get; set; } = "";
}
