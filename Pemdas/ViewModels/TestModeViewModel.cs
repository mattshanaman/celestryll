using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pemdas.Models;
using Pemdas.Services;
using System.Collections.ObjectModel;

namespace Pemdas.ViewModels;

public partial class TestModeViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly GameViewModel _gameViewModel;
    
    [ObservableProperty]
    private ObservableCollection<string> dayOptions = new()
    {
        "Monday (Easy - 1 value)",
        "Tuesday (Medium - Build It)",
        "Wednesday (Hard - 2 values)",
        "Thursday (Creative - Build It)",
        "Friday (Tricky - 3 values)",
        "Saturday (Speed - Build It)",
        "Sunday (Boss - 2 values)"
    };

    [ObservableProperty]
    private string selectedDay = "Monday (Easy - 1 value)";

    [ObservableProperty]
    private string previewPuzzle = string.Empty;

    [ObservableProperty]
    private string previewMode = string.Empty;

    [ObservableProperty]
    private string previewDifficulty = string.Empty;

    private DailyPuzzle? _currentTestPuzzle;

    public TestModeViewModel(DatabaseService databaseService, GameViewModel gameViewModel)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _gameViewModel = gameViewModel ?? throw new ArgumentNullException(nameof(gameViewModel));
        Title = "Test Mode - Puzzle Preview";

        // Load initial preview
        _ = LoadPreviewCommand.ExecuteAsync(null);
    }

    partial void OnSelectedDayChanged(string? oldValue, string newValue)
    {
        _ = LoadPreviewCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadPreview()
    {
        IsBusy = true;
        try
        {
            await _databaseService.Init();
            
            // Map selected day to difficulty/mode
            var (difficulty, mode) = SelectedDay switch
            {
                "Monday (Easy - 1 value)" => (DifficultyLevel.Easy, PuzzleMode.SolveIt),
                "Tuesday (Medium - Build It)" => (DifficultyLevel.Medium, PuzzleMode.BuildIt),
                "Wednesday (Hard - 2 values)" => (DifficultyLevel.Hard, PuzzleMode.SolveIt),
                "Thursday (Creative - Build It)" => (DifficultyLevel.Creative, PuzzleMode.BuildIt),
                "Friday (Tricky - 3 values)" => (DifficultyLevel.Tricky, PuzzleMode.SolveIt),
                "Saturday (Speed - Build It)" => (DifficultyLevel.Speed, PuzzleMode.BuildIt),
                "Sunday (Boss - 2 values)" => (DifficultyLevel.Boss, PuzzleMode.SolveIt),
                _ => (DifficultyLevel.Easy, PuzzleMode.SolveIt)
            };

            // Generate a sample puzzle for this type
            var samplePuzzle = GenerateSamplePuzzle(difficulty, mode);
            
            _currentTestPuzzle = samplePuzzle;
            PreviewPuzzle = samplePuzzle.PuzzleData;
            PreviewMode = mode == PuzzleMode.SolveIt ? "Solve It" : "Build It";
            PreviewDifficulty = difficulty.ToString();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading preview: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private DailyPuzzle GenerateSamplePuzzle(DifficultyLevel difficulty, PuzzleMode mode)
    {
        var random = new Random();
        
        if (mode == PuzzleMode.SolveIt)
        {
            return difficulty switch
            {
                // Easy: PEMDAS-focused without parentheses (Type A: Multiplication first)
                DifficultyLevel.Easy => new DailyPuzzle
                {
                    Mode = PuzzleMode.SolveIt,
                    Difficulty = DifficultyLevel.Easy,
                    PuzzleData = System.Text.Json.JsonSerializer.Serialize(new SolveItPuzzle
                    {
                        Equation = "? × 4 + 3 = 19",
                        BlankPositions = [0],
                        Solutions = [4],
                        AllowsExponents = false
                    }),
                    Solution = "4",
                    Hint = "Remember PEMDAS: Multiply before adding! Do ? × 4 first.",
                    BasePoints = 100,
                    TimeLimit = 0
                },
                
                // Hard: Natural PEMDAS without unnecessary parentheses
                DifficultyLevel.Hard => new DailyPuzzle
                {
                    Mode = PuzzleMode.SolveIt,
                    Difficulty = DifficultyLevel.Hard,
                    PuzzleData = System.Text.Json.JsonSerializer.Serialize(new SolveItPuzzle
                    {
                        Equation = "A × 2 + B ÷ 2 = 14",
                        BlankPositions = [0, 1],
                        Solutions = [5, 8],
                        AllowsExponents = false
                    }),
                    Solution = "5, 8",
                    Hint = "Remember PEMDAS: Do A × 2 and B ÷ 2 first (left-to-right), then add. A and B must be different. B must be even.",
                    BasePoints = 300,
                    TimeLimit = 0
                },
                
                // Tricky: Mixed operations teaching PEMDAS (Type A)
                DifficultyLevel.Tricky => new DailyPuzzle
                {
                    Mode = PuzzleMode.SolveIt,
                    Difficulty = DifficultyLevel.Tricky,
                    PuzzleData = System.Text.Json.JsonSerializer.Serialize(new SolveItPuzzle
                    {
                        Equation = "A + B × C = 30",
                        BlankPositions = [0, 1, 2],
                        Solutions = [18, 2, 6],
                        AllowsExponents = false
                    }),
                    Solution = "18, 2, 6",
                    Hint = "Remember PEMDAS: Do B × C first, then add A. All three values must be different integers.",
                    BasePoints = 400,
                    TimeLimit = 0
                },
                
                // Boss: Full PEMDAS chain (Type A: Exponent Priority)
                DifficultyLevel.Boss => new DailyPuzzle
                {
                    Mode = PuzzleMode.SolveIt,
                    Difficulty = DifficultyLevel.Boss,
                    PuzzleData = System.Text.Json.JsonSerializer.Serialize(new SolveItPuzzle
                    {
                        Equation = "X² + Y × 3 = 16",
                        BlankPositions = [0, 1],
                        Solutions = [2, 4],
                        AllowsExponents = true
                    }),
                    Solution = "2, 4",
                    Hint = "Remember PEMDAS: First calculate X² (exponent), then Y × 3 (multiply), finally add them. X and Y must be different.",
                    BasePoints = 500,
                    TimeLimit = 0
                },
                _ => new DailyPuzzle()
            };
        }
        else // Build It
        {
            return difficulty switch
            {
                // Medium: Increased difficulty - target 24 with 4 digits
                DifficultyLevel.Medium => new DailyPuzzle
                {
                    Mode = PuzzleMode.BuildIt,
                    Difficulty = DifficultyLevel.Medium,
                    PuzzleData = System.Text.Json.JsonSerializer.Serialize(new BuildItPuzzle
                    {
                        AvailableDigits = [2, 3, 4, 6],
                        TargetNumber = 24,
                        AcceptedSolutions = ["6 × 4 + 3 - 2", "3 × 6 + 4 + 2", "(2 + 4) × (6 - 3)"],
                        MaxParentheses = 1,
                        AllowsAdvancedOperators = false
                    }),
                    Solution = "6 × 4 + 3 - 2",
                    Hint = "One set of parentheses allowed. Bonus points for solutions without parentheses!",
                    BasePoints = 200,
                    TimeLimit = 0
                },
                
                // Creative: Increased difficulty - target 48 with challenging digits
                DifficultyLevel.Creative => new DailyPuzzle
                {
                    Mode = PuzzleMode.BuildIt,
                    Difficulty = DifficultyLevel.Creative,
                    PuzzleData = System.Text.Json.JsonSerializer.Serialize(new BuildItPuzzle
                    {
                        AvailableDigits = [3, 4, 6, 8],
                        TargetNumber = 48,
                        AcceptedSolutions = ["8 × 6 + 4 - 3", "6 × 8 + 3 - 4", "(3 + 4) × 6 - 8"],
                        MaxParentheses = 2,
                        AllowsAdvancedOperators = false
                    }),
                    Solution = "8 × 6 + 4 - 3",
                    Hint = "Elegant solutions (fewest symbols) earn extra points. Try solving without parentheses if possible!",
                    BasePoints = 350,
                    TimeLimit = 0
                },
                
                // Speed: Keep challenging with all digits
                DifficultyLevel.Speed => new DailyPuzzle
                {
                    Mode = PuzzleMode.BuildIt,
                    Difficulty = DifficultyLevel.Speed,
                    PuzzleData = System.Text.Json.JsonSerializer.Serialize(new BuildItPuzzle
                    {
                        AvailableDigits = [1, 2, 3, 4, 5, 6, 7, 8, 9],
                        TargetNumber = 45,
                        AcceptedSolutions = ["Multiple solutions possible"],
                        MaxParentheses = 3,
                        AllowsAdvancedOperators = false
                    }),
                    Solution = "Multiple solutions",
                    Hint = "Build equation in under 60 seconds. Multiple solutions accepted!",
                    BasePoints = 500,
                    TimeLimit = 60
                },
                _ => new DailyPuzzle()
            };
        }
    }

    [RelayCommand]
    private async Task NavigateToGame()
    {
        if (_currentTestPuzzle != null)
        {
            // Set the test puzzle in GameViewModel
            _gameViewModel.SetTestPuzzle(_currentTestPuzzle);
            
            // Navigate to game page
            await Shell.Current.GoToAsync("//game");
        }
    }
}
