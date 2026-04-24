using Pemdas.Models;
using System.Text.Json;

namespace Pemdas.Services
{
    public class GameService
    {
        private readonly DatabaseService _databaseService;
        private readonly ExpressionEvaluator _expressionEvaluator;
        private DateTime _gameStartTime;

        public GameService(DatabaseService databaseService, ExpressionEvaluator expressionEvaluator)
        {
            _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
            _expressionEvaluator = expressionEvaluator ?? throw new ArgumentNullException(nameof(expressionEvaluator));
        }

        public async Task<(DailyPuzzle? puzzle, bool alreadyPlayed)> GetTodaysPuzzle()
        {
            try
            {
                var puzzle = await _databaseService.GetTodaysPuzzle();
                var attempt = await _databaseService.GetTodaysAttempt();

                return (puzzle, attempt?.Solved ?? false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting today's puzzle: {ex.Message}");
                return (null, false);
            }
        }

        public void StartPuzzle()
        {
            _gameStartTime = DateTime.UtcNow;
        }

        public async Task<(bool isCorrect, int pointsEarned, bool streakBroken)> SubmitSolution(
            DailyPuzzle? puzzle,
            string? userSolution)
        {
            if (puzzle == null)
            {
                System.Diagnostics.Debug.WriteLine("Cannot submit solution: puzzle is null");
                return (false, 0, false);
            }

            if (string.IsNullOrWhiteSpace(userSolution))
            {
                System.Diagnostics.Debug.WriteLine("Cannot submit solution: user solution is empty");
                return (false, 0, false);
            }

            try
            {
                var timeSpent = (int)(DateTime.UtcNow - _gameStartTime).TotalSeconds;
                var isCorrect = ValidateSolution(puzzle, userSolution);

                var pointsEarned = 0;
                if (isCorrect)
                {
                    pointsEarned = CalculatePointsWithBonus(puzzle, userSolution, timeSpent);
                }

                var progress = await _databaseService.GetUserProgress();
                if (progress != null)
                {
                    var today = DateTime.UtcNow.Date;
                    var lastPlayed = progress.LastPlayedDate.Date;
                    var streakBroken = false;

                    if (isCorrect)
                    {
                        // Check if this is the first puzzle completed today (any difficulty counts)
                        var todaysAttempts = await _databaseService.GetAllTodaysAttempts();
                        var firstCompletionToday = !todaysAttempts.Any(a => a.Solved && a.AttemptDate.Date == today && a.PuzzleId != puzzle.Id);
                        
                        // Only update streak if this is the first puzzle completed today
                        if (firstCompletionToday)
                        {
                            if (lastPlayed == today.AddDays(-1))
                            {
                                progress.CurrentStreak++;

                                if (progress.CurrentStreak % 3 == 0)
                                {
                                    progress.HintTokens++;
                                }
                            }
                            else if (lastPlayed < today.AddDays(-1))
                            {
                                streakBroken = progress.CurrentStreak > 0;
                                progress.CurrentStreak = 1;
                            }
                            else if (lastPlayed != today)
                            {
                                progress.CurrentStreak = 1;
                            }

                            if (progress.CurrentStreak > progress.LongestStreak)
                            {
                                progress.LongestStreak = progress.CurrentStreak;
                            }
                        }

                        progress.TotalPuzzlesSolved++;
                        progress.TotalPoints += pointsEarned;
                    }

                    progress.LastPlayedDate = today;
                    await _databaseService.UpdateUserProgress(progress);

                    await _databaseService.SavePuzzleAttempt(new PuzzleAttempt
                    {
                        PuzzleId = puzzle.Id,
                        AttemptDate = DateTime.UtcNow,
                        Solved = isCorrect,
                        TimeSpent = timeSpent,
                        PointsEarned = pointsEarned,
                        UserSolution = userSolution,
                        AttemptsCount = 1,
                        DifficultySlot = puzzle.DifficultySlot
                    });

                    return (isCorrect, pointsEarned, streakBroken);
                }

                return (isCorrect, pointsEarned, false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error submitting solution: {ex.Message}");
                return (false, 0, false);
            }
        }

        public async Task<string?> UseHint(DailyPuzzle? puzzle)
        {
            if (puzzle == null)
            {
                System.Diagnostics.Debug.WriteLine("Cannot use hint: puzzle is null");
                return null;
            }

            try
            {
                var progress = await _databaseService.GetUserProgress();
                if (progress != null && progress.HintTokens > 0)
                {
                    progress.HintTokens--;
                    await _databaseService.UpdateUserProgress(progress);
                    return puzzle.Hint;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error using hint: {ex.Message}");
                return null;
            }
        }

        private bool ValidateSolution(DailyPuzzle puzzle, string userSolution)
        {
            try
            {
                if (puzzle.Mode == PuzzleMode.SolveIt)
                {
                    var solveItPuzzle = JsonSerializer.Deserialize<SolveItPuzzle>(puzzle.PuzzleData);
                    if (solveItPuzzle == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to deserialize Solve It puzzle");
                        return false;
                    }

                    // Parse user answer (e.g., "6, 4, 3" or "4")
                    var userValues = userSolution.Split(',')
                        .Select(v => v.Trim())
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Select(v => double.TryParse(v, out var num) ? num : double.NaN)
                        .ToList();

                    // Check if we have the right number of values
                    if (userValues.Count != solveItPuzzle.Solutions.Count)
                        return false;

                    // Check if any value failed to parse
                    if (userValues.Any(v => double.IsNaN(v)))
                        return false;

                    // Build the equation with user's values and evaluate
                    var equation = solveItPuzzle.Equation;
                    
                    // Replace placeholders with user values
                    if (solveItPuzzle.BlankPositions.Count == 1)
                    {
                        // Single value (?)
                        equation = equation.Replace("?", userValues[0].ToString());
                    }
                    else if (solveItPuzzle.BlankPositions.Count == 2)
                    {
                        // Two values (A, B or X, Y)
                        if (equation.Contains("X"))
                        {
                            equation = equation.Replace("X²", $"({userValues[0]}*{userValues[0]})");
                            equation = equation.Replace("X", userValues[0].ToString());
                            equation = equation.Replace("Y", userValues[1].ToString());
                        }
                        else
                        {
                            equation = equation.Replace("A", userValues[0].ToString());
                            equation = equation.Replace("B", userValues[1].ToString());
                        }
                    }
                    else if (solveItPuzzle.BlankPositions.Count == 3)
                    {
                        // Three values (A, B, C)
                        equation = equation.Replace("A", userValues[0].ToString());
                        equation = equation.Replace("B", userValues[1].ToString());
                        equation = equation.Replace("C", userValues[2].ToString());
                    }

                    // Extract the target value from equation (after =)
                    var parts = equation.Split('=');
                    if (parts.Length != 2)
                        return false;

                    var leftSide = parts[0].Trim();
                    var rightSide = parts[1].Trim();

                    if (!double.TryParse(rightSide, out var target))
                        return false;

                    // Replace operators for evaluation
                    leftSide = leftSide.Replace("×", "*")
                                     .Replace("÷", "/")
                                     .Replace("−", "-");

                    var (isValid, result) = _expressionEvaluator.Evaluate(leftSide);
                    
                    if (!isValid)
                        return false;

                    // Check if result matches target
                    return Math.Abs(result - target) < 0.001;
                }
                else // Build It
                {
                    var buildItPuzzle = JsonSerializer.Deserialize<BuildItPuzzle>(puzzle.PuzzleData);
                    if (buildItPuzzle == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to deserialize Build It puzzle");
                        return false;
                    }

                    if (!_expressionEvaluator.ValidateDigitsUsed(userSolution, buildItPuzzle.AvailableDigits))
                    {
                        System.Diagnostics.Debug.WriteLine("Invalid digits used in solution");
                        return false;
                    }

                    if (_expressionEvaluator.CountParentheses(userSolution) > buildItPuzzle.MaxParentheses)
                    {
                        System.Diagnostics.Debug.WriteLine("Too many parentheses in solution");
                        return false;
                    }

                    var (isValid, result) = _expressionEvaluator.Evaluate(userSolution);
                    if (!isValid)
                    {
                        System.Diagnostics.Debug.WriteLine("Expression evaluation failed");
                        return false;
                    }

                    return Math.Abs(result - buildItPuzzle.TargetNumber) < 0.001;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error validating solution: {ex.Message}");
                return false;
            }
        }

        private int CalculatePoints(DailyPuzzle puzzle, int timeSpent)
        {
            try
            {
                var basePoints = puzzle.BasePoints;

                // Time bonus for Speed mode
                if (puzzle.TimeLimit > 0 && timeSpent <= puzzle.TimeLimit)
                {
                    var timeBonus = Math.Max(0, (puzzle.TimeLimit - timeSpent) * 2);
                    basePoints += timeBonus;
                }

                return basePoints;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating points: {ex.Message}");
                return puzzle.BasePoints;
            }
        }

        // New method to calculate bonus points for elegant solutions
        public int CalculatePointsWithBonus(DailyPuzzle puzzle, string userSolution, int timeSpent)
        {
            try
            {
                var basePoints = CalculatePoints(puzzle, timeSpent);

                // Build It mode: Bonus for parentheses-free solutions
                if (puzzle.Mode == PuzzleMode.BuildIt)
                {
                    var buildItPuzzle = JsonSerializer.Deserialize<BuildItPuzzle>(puzzle.PuzzleData);
                    if (buildItPuzzle != null)
                    {
                        var parenthesesCount = _expressionEvaluator.CountParentheses(userSolution);
                        
                        // Award bonus points for elegant solutions
                        if (parenthesesCount == 0 && buildItPuzzle.MaxParentheses > 0)
                        {
                            // 50 point bonus for solving without parentheses when they're allowed
                            basePoints += 50;
                            System.Diagnostics.Debug.WriteLine("Bonus: +50 points for parentheses-free solution!");
                        }
                        else if (parenthesesCount < buildItPuzzle.MaxParentheses)
                        {
                            // 25 point bonus for using fewer parentheses than allowed
                            basePoints += 25;
                            System.Diagnostics.Debug.WriteLine("Bonus: +25 points for elegant solution!");
                        }
                    }
                }

                return basePoints;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calculating bonus points: {ex.Message}");
                return puzzle.BasePoints;
            }
        }

        public string GenerateShareableResult(DailyPuzzle? puzzle, bool solved, int attempts)
        {
            if (puzzle == null)
                return "PEMDAS Daily Challenge";

            try
            {
                var emoji = solved ? "✅" : "❌";
                var difficultyEmoji = puzzle.Difficulty switch
                {
                    DifficultyLevel.Easy => "⭐",
                    DifficultyLevel.Medium => "⭐⭐",
                    DifficultyLevel.Hard => "⭐⭐⭐",
                    DifficultyLevel.Creative => "🎨",
                    DifficultyLevel.Tricky => "🧩",
                    DifficultyLevel.Speed => "⚡",
                    DifficultyLevel.Boss => "👑",
                    _ => "⭐"
                };

                return $"PEMDAS {puzzle.PuzzleDate:yyyy-MM-dd} {difficultyEmoji}\n" +
                       $"{emoji} {puzzle.Mode} Mode\n" +
                       $"Attempts: {attempts}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating shareable result: {ex.Message}");
                return "PEMDAS Daily Challenge";
            }
        }
    }
}