using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pemdas.Models;
using Pemdas.Resources.Localization;
using Pemdas.Services;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Pemdas.ViewModels;

public partial class GameViewModel : BaseViewModel
{
    private readonly GameService _gameService;
    private readonly DatabaseService _databaseService;
    private readonly IAdService _adService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IFeedbackService _feedbackService;
    private bool _hasVerifiedDatabaseIntegrity;
    private DailyPuzzle? _currentPuzzle;
    private int _attemptsCount;
    private IDispatcherTimer? _timer;

    [ObservableProperty]
    private string puzzleDisplay = string.Empty;

    [ObservableProperty]
    private string userInput = string.Empty;

    [ObservableProperty]
    private string modeText = string.Empty;

    [ObservableProperty]
    private string difficultyText = string.Empty;

    [ObservableProperty]
    private string hintText = string.Empty;

    [ObservableProperty]
    private bool showHint;

    [ObservableProperty]
    private string feedbackMessage = string.Empty;

    [ObservableProperty]
    private bool showFeedback;

    [ObservableProperty]
    private int currentStreak;

    [ObservableProperty]
    private int hintTokens;

    [ObservableProperty]
    private bool puzzleCompleted;

    [ObservableProperty]
    private string timeRemaining = string.Empty;

    [ObservableProperty]
    private bool hasTimeLimit;

    [ObservableProperty]
    private ObservableCollection<string> availableDigits = new();

    [ObservableProperty]
    private string availableDigitsDisplay = string.Empty;

    [ObservableProperty]
    private string targetNumber = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string inputHelpText = string.Empty;

    [ObservableProperty]
    private bool showInputHelp;

    [ObservableProperty]
    private bool showSingleInput = true;

    [ObservableProperty]
    private bool showMultiInput;
    
    // Manual property for ShowCalculator to ensure it works
    private bool _showCalculator;
    public bool ShowCalculator
    {
        get => _showCalculator;
        set => SetProperty(ref _showCalculator, value);
    }

    [ObservableProperty]
    private bool showValueA;

    [ObservableProperty]
    private bool showValueB;

    [ObservableProperty]
    private bool showValueC;

    [ObservableProperty]
    private string labelA = "A =";

    [ObservableProperty]
    private string labelB = "B =";

    [ObservableProperty]
    private string labelC = "C =";

    [ObservableProperty]
    private string valueA = string.Empty;

    [ObservableProperty]
    private string valueB = string.Empty;

    [ObservableProperty]
    private string valueC = string.Empty;

    [ObservableProperty]
    private bool isTestMode;

    [ObservableProperty]
    private bool isArchiveMode;

    [ObservableProperty]
    private bool digit0Enabled = true;

    [ObservableProperty]
    private bool digit1Enabled = true;

    [ObservableProperty]
    private bool digit2Enabled = true;

    [ObservableProperty]
    private bool digit3Enabled = true;

    [ObservableProperty]
    private bool digit4Enabled = true;

    [ObservableProperty]
    private bool digit5Enabled = true;

    [ObservableProperty]
    private bool digit6Enabled = true;

    [ObservableProperty]
    private bool digit7Enabled = true;

    [ObservableProperty]
    private bool digit8Enabled = true;

    [ObservableProperty]
    private bool digit9Enabled = true;
    
    // Difficulty selection and premium features
    [ObservableProperty]
    private int selectedDifficultySlot = 0; // 0=Easy, 1=Medium, etc.
    
    [ObservableProperty]
    private bool isSubscribed = false;
    
    [ObservableProperty]
    private bool hasWatchedAdToday = false;
    
    [ObservableProperty]
    private bool canSelectDifficulty = false;
    
    [ObservableProperty]
    private bool showDifficultySelector = true;
    
    // Individual difficulty button states
    [ObservableProperty]
    private bool easyEnabled = true;
    
    [ObservableProperty]
    private bool mediumEnabled = false;
    
    [ObservableProperty]
    private bool hardEnabled = false;
    
    [ObservableProperty]
    private bool creativeEnabled = false;
    
    [ObservableProperty]
    private bool trickyEnabled = false;
    
    [ObservableProperty]
    private bool speedEnabled = false;
    
    [ObservableProperty]
    private bool bossEnabled = false;
    
    [ObservableProperty]
    private bool expertEnabled = false;

    private DailyPuzzle? _testPuzzle; // For test mode
    
    // Time tracking
    private DateTime _puzzleStartTime;
    private int _completionTimeSeconds;
    private IDispatcherTimer? _elapsedTimer;
    
    private string _elapsedTime = "0:00";
    public string ElapsedTime
    {
        get => _elapsedTime;
        set => SetProperty(ref _elapsedTime, value);
    }
    
    public string CompletionTimeDisplay => 
        _completionTimeSeconds > 0 
            ? $"{_completionTimeSeconds / 60}:{_completionTimeSeconds % 60:D2}"
            : "0:00";
    
    // Puzzle identification and completion tracking
    private string _currentPuzzleId = string.Empty;
    public string CurrentPuzzleId
    {
        get => _currentPuzzleId;
        set => SetProperty(ref _currentPuzzleId, value);
    }
    
    private ObservableCollection<int> _completedDifficulties = new();
    public ObservableCollection<int> CompletedDifficulties
    {
        get => _completedDifficulties;
        set => SetProperty(ref _completedDifficulties, value);
    }
    
    private string _completedDifficultiesDisplay = string.Empty;
    public string CompletedDifficultiesDisplay
    {
        get => _completedDifficultiesDisplay;
        set => SetProperty(ref _completedDifficultiesDisplay, value);
    }
    
    private bool _hasCompletedThisDifficulty;
    public bool HasCompletedThisDifficulty
    {
        get => _hasCompletedThisDifficulty;
        set => SetProperty(ref _hasCompletedThisDifficulty, value);
    }
    
    private string _completionMessage = string.Empty;
    public string CompletionMessage
    {
        get => _completionMessage;
        set => SetProperty(ref _completionMessage, value);
    }
    
    private int _totalPointsEarned;
    public int TotalPointsEarned
    {
        get => _totalPointsEarned;
        set => SetProperty(ref _totalPointsEarned, value);
    }
    
    // Ad requirements
    private bool _requiresAd;
    public bool RequiresAd
    {
        get => _requiresAd;
        set => SetProperty(ref _requiresAd, value);
    }

    public GameViewModel(
        GameService gameService,
        DatabaseService databaseService,
        IAdService adService,
        ISubscriptionService subscriptionService,
        IFeedbackService feedbackService)
    {
        _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _adService = adService ?? throw new ArgumentNullException(nameof(adService));
        _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
        _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
        Title = AppResources.TabDailyChallenge;
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        HasError = false;
        ShowFeedback = false; // Clear any previous feedback messages

        try
        {
            System.Diagnostics.Debug.WriteLine("🚀 GameViewModel.InitializeAsync started");
            
            // Verify database integrity first
            if (!_hasVerifiedDatabaseIntegrity)
            {
                var dbIntegrity = await _databaseService.VerifyDatabaseIntegrity();
                if (!dbIntegrity)
                {
                    System.Diagnostics.Debug.WriteLine("⚠️ Database integrity check failed - may need regeneration");
                }

                _hasVerifiedDatabaseIntegrity = true;
            }
            
            // Update subscription status and difficulty buttons (with error handling)
            try
            {
                await UpdateSubscriptionStatus();
            }
            catch (Exception subEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating subscription status: {subEx.Message}");
                // Set safe defaults if subscription check fails
                IsSubscribed = false;
                CanSelectDifficulty = false;
                UpdateDifficultyButtons();
            }
            
            // WORDLE-STYLE: Show ad before puzzle loads (unless subscribed or test mode)
            if (!IsSubscribed && !IsTestMode && !IsArchiveMode)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("📺 Showing Wordle-style interstitial ad before puzzle...");
                    _adService.ShowInterstitialAd();

                    // Brief delay to let ad display
                    await Task.Delay(500);
                }
                catch (Exception adEx)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Error showing ad: {adEx.Message}");
                    // Continue anyway - don't block puzzle access if ad fails
                }
            }

            // Check if archive puzzle is set - allow ad-free premium replay of past games
            if (_testPuzzle != null && IsArchiveMode)
            {
                _currentPuzzle = _testPuzzle;
                PuzzleCompleted = false;
                _gameService.StartPuzzle();
                SetupPuzzle(_testPuzzle);
                await UpdateUserProgress();
                IsBusy = false;
                return;
            }
            
            // Check if test puzzle is set (test mode) - if so, just use it without reloading
            if (_testPuzzle != null && IsTestMode)
            {
                // Test puzzle already set, just setup the UI
                _currentPuzzle = _testPuzzle;
                PuzzleCompleted = false; // Always allow play in test mode
                _gameService.StartPuzzle();
                SetupPuzzle(_testPuzzle);
                await UpdateUserProgress();
                IsBusy = false;
                return;
            }

            var (puzzle, alreadyPlayed) = await _gameService.GetTodaysPuzzle();

            if (puzzle != null)
            {
                // Check if puzzle needs regeneration (old notation or old PEMDAS format)
                var needsRegeneration = false;
                
                // Check 1: Old underscore notation
                if (puzzle.PuzzleData.Contains("_"))
                {
                    System.Diagnostics.Debug.WriteLine("Old puzzle notation detected (underscore)...");
                    needsRegeneration = true;
                }
                
                // Check 2: Old PEMDAS format (Easy mode with unnecessary parentheses)
                if (!needsRegeneration && puzzle.Mode == PuzzleMode.SolveIt && puzzle.Difficulty == DifficultyLevel.Easy)
                {
                    var solveItPuzzle = JsonSerializer.Deserialize<SolveItPuzzle>(puzzle.PuzzleData);
                    if (solveItPuzzle != null && solveItPuzzle.Equation.StartsWith("(? +") && solveItPuzzle.Equation.Contains(")"))
                    {
                        System.Diagnostics.Debug.WriteLine("Old PEMDAS format detected (unnecessary parentheses)...");
                        needsRegeneration = true;
                    }
                }
                
                // Check 3: Old Boss puzzle format (may have same X,Y values)
                if (!needsRegeneration && puzzle.Mode == PuzzleMode.SolveIt && puzzle.Difficulty == DifficultyLevel.Boss)
                {
                    var solveItPuzzle = JsonSerializer.Deserialize<SolveItPuzzle>(puzzle.PuzzleData);
                    if (solveItPuzzle != null && solveItPuzzle.Equation.StartsWith("(X²") && solveItPuzzle.Equation.Contains(") ÷ ("))
                    {
                        System.Diagnostics.Debug.WriteLine("Old Boss format detected (excessive parentheses)...");
                        needsRegeneration = true;
                    }
                }
                
                if (needsRegeneration)
                {
                    System.Diagnostics.Debug.WriteLine("Regenerating all puzzles with new PEMDAS-focused format...");
                    await _databaseService.ClearAndRegeneratePuzzles();
                    
                    // Get the new puzzle
                    (puzzle, alreadyPlayed) = await _gameService.GetTodaysPuzzle();
                    System.Diagnostics.Debug.WriteLine("Puzzles regenerated successfully!");
                }

                if (puzzle != null)
                {
                    _currentPuzzle = puzzle;
                    PuzzleCompleted = alreadyPlayed;

                    // Always setup the puzzle to display it
                    _gameService.StartPuzzle();
                    SetupPuzzle(puzzle);
                    
                    // Initialize time tracking and completion info
                    await InitializeCompletionTracking();
                    
                    if (alreadyPlayed)
                    {
                        FeedbackMessage = AppResources.AlreadyCompleted;
                        ShowFeedback = true;
                    }
                    else
                    {
                        // Start elapsed time timer
                        StartElapsedTimer();
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("❌ PUZZLE IS NULL - No puzzle loaded from database!");
                System.Diagnostics.Debug.WriteLine("📊 Possible causes:");
                System.Diagnostics.Debug.WriteLine("   1. Database not initialized");
                System.Diagnostics.Debug.WriteLine("   2. Today's puzzle missing");
                System.Diagnostics.Debug.WriteLine("   3. Wrong difficulty slot selected");
                
                ErrorMessage = "Failed to load today's puzzle.\n\nTry:\n• Restart the app\n• Check Debug output\n• Clear app data if problem persists";
                HasError = true;
                await _feedbackService.PlayErrorFeedback();
            }

            await UpdateUserProgress();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error initializing game: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"   Stack: {ex.StackTrace}");
            
            ErrorMessage = $"Error loading puzzle:\n{ex.Message}\n\nCheck Debug output for details.";
            HasError = true;
            await _feedbackService.PlayErrorFeedback();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void SetupPuzzle(DailyPuzzle puzzle)
    {
        try
        {
            var modeString = puzzle.Mode == PuzzleMode.SolveIt ? AppResources.SolveIt : AppResources.BuildIt;
            ModeText = $"{AppResources.Mode}: {modeString}";
            
            var difficultyString = puzzle.Difficulty switch
            {
                DifficultyLevel.Easy => AppResources.Easy,
                DifficultyLevel.Medium => AppResources.Medium,
                DifficultyLevel.Hard => AppResources.Hard,
                DifficultyLevel.Creative => AppResources.Creative,
                DifficultyLevel.Tricky => AppResources.Tricky,
                DifficultyLevel.Speed => AppResources.Speed,
                DifficultyLevel.Boss => AppResources.Boss,
                DifficultyLevel.Expert => "Expert", // Add localization later
                _ => AppResources.Easy
            };
            DifficultyText = $"{AppResources.Difficulty}: {difficultyString}";
            HasTimeLimit = puzzle.TimeLimit > 0;

            if (puzzle.Mode == PuzzleMode.SolveIt)
            {
                // Enable all digits for Solve It mode
                Digit0Enabled = Digit1Enabled = Digit2Enabled = Digit3Enabled = Digit4Enabled = 
                Digit5Enabled = Digit6Enabled = Digit7Enabled = Digit8Enabled = Digit9Enabled = true;
                
                // Solve It only needs simple numeric input, no calculator
                ShowCalculator = false;
                
                var solveItPuzzle = JsonSerializer.Deserialize<SolveItPuzzle>(puzzle.PuzzleData);
                if (solveItPuzzle != null)
                {
                    PuzzleDisplay = solveItPuzzle.Equation;
                    
                    // Configure input display based on number of blanks
                    var blankCount = solveItPuzzle.BlankPositions?.Count ?? 1;
                    ConfigureInputDisplay(blankCount, puzzle.Difficulty);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to deserialize Solve It puzzle");
                    PuzzleDisplay = "Error loading puzzle";
                    ShowSingleInput = true;
                    ShowMultiInput = false;
                    ShowInputHelp = false;
                }
            }
            else
            {
                var buildItPuzzle = JsonSerializer.Deserialize<BuildItPuzzle>(puzzle.PuzzleData);
                if (buildItPuzzle != null)
                {
                    AvailableDigits = new ObservableCollection<string>(
                        buildItPuzzle.AvailableDigits.Select(d => d.ToString()));
                    AvailableDigitsDisplay = string.Join(", ", buildItPuzzle.AvailableDigits);
                    TargetNumber = buildItPuzzle.TargetNumber.ToString();
                    
                    // Configure which digit buttons are enabled
                    Digit0Enabled = buildItPuzzle.AvailableDigits.Contains(0);
                    Digit1Enabled = buildItPuzzle.AvailableDigits.Contains(1);
                    Digit2Enabled = buildItPuzzle.AvailableDigits.Contains(2);
                    Digit3Enabled = buildItPuzzle.AvailableDigits.Contains(3);
                    Digit4Enabled = buildItPuzzle.AvailableDigits.Contains(4);
                    Digit5Enabled = buildItPuzzle.AvailableDigits.Contains(5);
                    Digit6Enabled = buildItPuzzle.AvailableDigits.Contains(6);
                    Digit7Enabled = buildItPuzzle.AvailableDigits.Contains(7);
                    Digit8Enabled = buildItPuzzle.AvailableDigits.Contains(8);
                    Digit9Enabled = buildItPuzzle.AvailableDigits.Contains(9);
                    
                    // Try to use localized string, fallback to English if not available
                    try
                    {
                        PuzzleDisplay = string.Format(AppResources.BuildEquation, buildItPuzzle.TargetNumber);
                    }
                    catch
                    {
                        PuzzleDisplay = $"Build equation to reach: {buildItPuzzle.TargetNumber}";
                    }
                    
                    // Build It puzzles need calculator to build expressions
                    ShowSingleInput = true;
                    ShowMultiInput = false;
                    ShowInputHelp = true;
                    ShowCalculator = true; // Show calculator for Build It mode
                    InputHelpText = "Build equation using all digits (e.g., (1 + 3) × 2 + 4)";
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to deserialize Build It puzzle");
                    PuzzleDisplay = "Error loading puzzle";
                    ShowSingleInput = true;
                    ShowMultiInput = false;
                    ShowInputHelp = false;
                    ShowCalculator = false;
                }
            }

            if (HasTimeLimit)
            {
                StartTimer(puzzle.TimeLimit);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error setting up puzzle: {ex.Message}");
            PuzzleDisplay = "Error displaying puzzle";
            ShowSingleInput = true;
            ShowMultiInput = false;
            ShowInputHelp = false;
        }
    }

    private void ConfigureInputDisplay(int blankCount, DifficultyLevel difficulty)
    {
        // Clear previous values
        ValueA = string.Empty;
        ValueB = string.Empty;
        ValueC = string.Empty;
        
        if (blankCount == 1)
        {
            // Single value - use simple input
            ShowSingleInput = true;
            ShowMultiInput = false;
            ShowInputHelp = false;
        }
        else
        {
            // Multiple values - use labeled boxes
            ShowSingleInput = false;
            ShowMultiInput = true;
            ShowInputHelp = true;
            
            // Set labels based on difficulty (determines variable names)
            if (difficulty == DifficultyLevel.Boss)
            {
                LabelA = "X =";
                LabelB = "Y =";
                ShowValueA = true;
                ShowValueB = true;
                ShowValueC = false;
                InputHelpText = "Enter value for X and Y (negative values allowed)";
            }
            else if (blankCount == 2)
            {
                LabelA = "A =";
                LabelB = "B =";
                ShowValueA = true;
                ShowValueB = true;
                ShowValueC = false;
                InputHelpText = "Enter value for A and B (negative values allowed)";
            }
            else if (blankCount == 3)
            {
                LabelA = "A =";
                LabelB = "B =";
                LabelC = "C =";
                ShowValueA = true;
                ShowValueB = true;
                ShowValueC = true;
                InputHelpText = "Enter value for A, B, and C (negative values allowed)";
            }
        }
    }

    private void StartTimer(int seconds)
    {
        try
        {
            _timer?.Stop();
            
            _timer = Application.Current?.Dispatcher.CreateTimer();
            if (_timer != null)
            {
                var endTime = DateTime.UtcNow.AddSeconds(seconds);
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += (s, e) =>
                {
                    try
                    {
                        var remaining = (endTime - DateTime.UtcNow).TotalSeconds;
                        if (remaining <= 0)
                        {
                            TimeRemaining = AppResources.TimeUp;
                            _timer?.Stop();
                            PuzzleCompleted = true;
                            _ = _feedbackService.PlayErrorFeedback();
                        }
                        else
                        {
                            TimeRemaining = $"{AppResources.Time}: {(int)remaining}s";
                            
                            // Play countdown sound when time is low
                            if (remaining <= 10 && remaining % 1 < 0.1)
                            {
                                _ = _feedbackService.PlayCountdownSound();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in timer tick: {ex.Message}");
                    }
                };
                _timer.Start();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error starting timer: {ex.Message}");
        }
    }
    
    private void StartElapsedTimer()
    {
        try
        {
            _puzzleStartTime = DateTime.UtcNow;
            _elapsedTimer?.Stop();
            
            _elapsedTimer = Application.Current?.Dispatcher.CreateTimer();
            if (_elapsedTimer != null)
            {
                _elapsedTimer.Interval = TimeSpan.FromSeconds(1);
                _elapsedTimer.Tick += (s, e) =>
                {
                    try
                    {
                        var elapsed = DateTime.UtcNow - _puzzleStartTime;
                        ElapsedTime = $"{(int)elapsed.TotalMinutes}:{elapsed.Seconds:D2}";
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in elapsed timer: {ex.Message}");
                    }
                };
                _elapsedTimer.Start();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error starting elapsed timer: {ex.Message}");
        }
    }
    
    private void StopElapsedTimer()
    {
        _elapsedTimer?.Stop();
        var elapsed = DateTime.UtcNow - _puzzleStartTime;
        _completionTimeSeconds = (int)elapsed.TotalSeconds;
    }
    
    private async Task InitializeCompletionTracking()
    {
        try
        {
            if (_currentPuzzle == null) return;
            
            // Set puzzle ID
            CurrentPuzzleId = _currentPuzzle.PuzzleIdentifier;
            
            // Get completed difficulties for today
            var completed = await _databaseService.GetTodaysCompletedDifficulties();
            CompletedDifficulties = new ObservableCollection<int>(completed);
            
            // Update display of completed difficulties
            UpdateCompletedDifficultiesDisplay();
            
            // Check if this specific difficulty is completed
            HasCompletedThisDifficulty = CompletedDifficulties.Contains(_currentPuzzle.DifficultySlot);
            
            // Get total points
            TotalPointsEarned = await _databaseService.GetTotalPointsEarned();
            
            // Update completion message
            UpdateCompletionMessage();
            
            // Check if ad required (if not first completion and not subscribed)
            var isSubscribed = await _subscriptionService.CheckSubscriptionStatus();
            RequiresAd = CompletedDifficulties.Count > 0 && !isSubscribed;
            
            System.Diagnostics.Debug.WriteLine($"📊 Completion tracking initialized:");
            System.Diagnostics.Debug.WriteLine($"   Puzzle ID: {CurrentPuzzleId}");
            System.Diagnostics.Debug.WriteLine($"   Completed today: {CompletedDifficulties.Count}");
            System.Diagnostics.Debug.WriteLine($"   Total points: {TotalPointsEarned}");
            System.Diagnostics.Debug.WriteLine($"   Requires ad: {RequiresAd}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing completion tracking: {ex.Message}");
        }
    }
    
    private void UpdateCompletedDifficultiesDisplay()
    {
        if (CompletedDifficulties.Count == 0)
        {
            CompletedDifficultiesDisplay = "None yet";
            return;
        }
        
        var icons = new List<string>();
        foreach (var slot in CompletedDifficulties.OrderBy(s => s))
        {
            var icon = slot switch
            {
                0 => "⭐",
                1 => "⭐⭐",
                2 => "⭐⭐⭐",
                3 => "🎨",
                4 => "🧩",
                5 => "⚡",
                6 => "👑",
                7 => "🎓",
                _ => "?"
            };
            icons.Add(icon);
        }
        
        CompletedDifficultiesDisplay = string.Join(" ", icons);
    }
    
    private void UpdateCompletionMessage()
    {
        if (HasCompletedThisDifficulty)
        {
            var remaining = 8 - CompletedDifficulties.Count;
            CompletionMessage = remaining > 0
                ? $"Try the remaining {remaining} difficulty levels!"
                : "Amazing! You've completed all 8 difficulties today! 🎉";
        }
        else
        {
            CompletionMessage = string.Empty;
        }
    }

    [RelayCommand]
    private async Task SubmitAnswer()
    {
        // Combine values based on input type
        string userAnswer;
        if (ShowSingleInput)
        {
            userAnswer = UserInput;
        }
        else
        {
            // Combine multi-value inputs with comma separation
            var values = new List<string>();
            if (ShowValueA && !string.IsNullOrWhiteSpace(ValueA))
                values.Add(ValueA.Trim());
            if (ShowValueB && !string.IsNullOrWhiteSpace(ValueB))
                values.Add(ValueB.Trim());
            if (ShowValueC && !string.IsNullOrWhiteSpace(ValueC))
                values.Add(ValueC.Trim());
                
            userAnswer = string.Join(", ", values);
        }
        
        if (string.IsNullOrWhiteSpace(userAnswer))
        {
            FeedbackMessage = AppResources.PleaseEnterAnswer;
            ShowFeedback = true;
            await _feedbackService.PlayErrorFeedback();
            return;
        }

        if (_currentPuzzle == null)
        {
            FeedbackMessage = AppResources.NoPuzzleLoaded;
            ShowFeedback = true;
            await _feedbackService.PlayErrorFeedback();
            return;
        }

        IsBusy = true;
        _attemptsCount++;
        HasError = false;

        try
        {
            // In test mode, just validate the answer without saving progress
            if (IsTestMode || IsArchiveMode)
            {
                var testIsCorrect = ValidateTestAnswer(userAnswer);
                
                if (testIsCorrect)
                {
                    var modeLabel = IsArchiveMode ? "Past Game" : "Test Mode";
                    FeedbackMessage = $"✅ Correct! ({modeLabel} - no points awarded)";
                    ShowFeedback = true;
                    PuzzleCompleted = true;
                    _timer?.Stop();
                    await _feedbackService.PlaySuccessFeedback();
                }
                else
                {
                    var modeLabel = IsArchiveMode ? "Past Game" : "Test Mode";
                    FeedbackMessage = $"❌ Not quite right. Try again! ({modeLabel})";
                    ShowFeedback = true;
                    await _feedbackService.PlayErrorFeedback();
                }
                
                IsBusy = false;
                return;
            }

            var (isCorrect, pointsEarned, streakBroken) =
                await _gameService.SubmitSolution(_currentPuzzle, userAnswer);

            if (isCorrect)
            {
                // Stop timer and calculate completion time
                StopElapsedTimer();
                
                // Check for PEMDAS bonus (Build It mode only)
                string bonusMessage = string.Empty;
                if (_currentPuzzle.Mode == PuzzleMode.BuildIt)
                {
                    var buildItPuzzle = JsonSerializer.Deserialize<BuildItPuzzle>(_currentPuzzle.PuzzleData);
                    if (buildItPuzzle != null)
                    {
                        var evaluator = new ExpressionEvaluator();
                        var parenthesesCount = evaluator.CountParentheses(userAnswer);
                        
                        if (parenthesesCount == 0 && buildItPuzzle.MaxParentheses > 0)
                        {
                            bonusMessage = " 🎯 +50 PEMDAS Bonus!";
                        }
                        else if (parenthesesCount < buildItPuzzle.MaxParentheses)
                        {
                            bonusMessage = " ⭐ +25 Elegant Solution!";
                        }
                    }
                }
                
                // Record completion
                bool recorded = await _databaseService.RecordDailyCompletion(
                    _currentPuzzle.DifficultySlot,
                    _currentPuzzle.PuzzleIdentifier,
                    _completionTimeSeconds,
                    pointsEarned,
                    !IsSubscribed // Wordle-style: all non-subscribers see ads
                );
                
                if (recorded)
                {
                    // Update completed list
                    if (!CompletedDifficulties.Contains(_currentPuzzle.DifficultySlot))
                    {
                        CompletedDifficulties.Add(_currentPuzzle.DifficultySlot);
                        UpdateCompletedDifficultiesDisplay();
                    }
                    
                    // Update total points
                    TotalPointsEarned = await _databaseService.GetTotalPointsEarned();
                    
                    // Update completion message
                    HasCompletedThisDifficulty = true;
                    UpdateCompletionMessage();
                    
                    System.Diagnostics.Debug.WriteLine($"✅ Completion recorded: {_currentPuzzle.PuzzleIdentifier}");
                    System.Diagnostics.Debug.WriteLine($"   Time: {CompletionTimeDisplay}");
                    System.Diagnostics.Debug.WriteLine($"   Points: {pointsEarned}");
                    System.Diagnostics.Debug.WriteLine($"   Total points: {TotalPointsEarned}");
                }
                
                FeedbackMessage = $"Correct! +{pointsEarned} points! Time: {CompletionTimeDisplay}{bonusMessage}";
                ShowFeedback = true;
                PuzzleCompleted = true;
                _timer?.Stop();

                // Success feedback with streak bonus
                await _feedbackService.PlaySuccessFeedback();
                if (CurrentStreak > 0 && !streakBroken)
                {
                    await Task.Delay(300);
                    await _feedbackService.PlayStreakFeedback();
                }

                try
                {
                    if (!await _subscriptionService.CheckSubscriptionStatus())
                    {
                        _adService.ShowInterstitialAd();
                    }
                }
                catch (Exception adEx)
                {
                    System.Diagnostics.Debug.WriteLine($"Error showing ad: {adEx.Message}");
                }
            }
            else
            {
                FeedbackMessage = AppResources.WrongAnswer;
                ShowFeedback = true;
                await _feedbackService.PlayErrorFeedback();
            }

            await UpdateUserProgress();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error submitting answer: {ex.Message}");
            FeedbackMessage = AppResources.ErrorCheckingAnswer;
            ShowFeedback = true;
            HasError = true;
            await _feedbackService.PlayErrorFeedback();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool ValidateTestAnswer(string userAnswer)
    {
        if (_currentPuzzle == null)
            return false;

        try
        {
            // For Build It puzzles, use mathematical validation
            if (_currentPuzzle.Mode == PuzzleMode.BuildIt)
            {
                var buildItPuzzle = JsonSerializer.Deserialize<BuildItPuzzle>(_currentPuzzle.PuzzleData);
                if (buildItPuzzle == null)
                    return false;

                // Validate the expression mathematically
                var evaluator = new ExpressionEvaluator();
                var (isValid, result) = evaluator.Evaluate(userAnswer);
                
                if (!isValid)
                    return false;

                // Check if result matches target
                return Math.Abs(result - buildItPuzzle.TargetNumber) < 0.001;
            }
            else // Solve It puzzles
            {
                var solveItPuzzle = JsonSerializer.Deserialize<SolveItPuzzle>(_currentPuzzle.PuzzleData);
                if (solveItPuzzle == null)
                    return false;

                // Parse user answer (e.g., "6, 4, 3" or "4")
                var userValues = userAnswer.Split(',')
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

                // Evaluate the left side
                var evaluator = new ExpressionEvaluator();
                
                // Replace operators for evaluation
                leftSide = leftSide.Replace("×", "*")
                                 .Replace("÷", "/")
                                 .Replace("−", "-");

                var (isValid, result) = evaluator.Evaluate(leftSide);
                
                if (!isValid)
                    return false;

                // Check if result matches target
                return Math.Abs(result - target) < 0.001;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error validating test answer: {ex.Message}");
            return false;
        }
    }

    [RelayCommand]
    private async Task UseHint()
    {
        if (_currentPuzzle == null)
        {
            FeedbackMessage = AppResources.NoPuzzleLoaded;
            ShowFeedback = true;
            await _feedbackService.PlayErrorFeedback();
            return;
        }

        try
        {
            var hint = await _gameService.UseHint(_currentPuzzle);
            if (hint != null)
            {
                HintText = hint;
                ShowHint = true;
                await _feedbackService.PlayHintFeedback();
                await UpdateUserProgress();
            }
            else
            {
                FeedbackMessage = AppResources.NoHintTokens;
                ShowFeedback = true;
                await _feedbackService.PlayErrorFeedback();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error using hint: {ex.Message}");
            FeedbackMessage = AppResources.ErrorGettingHint;
            ShowFeedback = true;
            await _feedbackService.PlayErrorFeedback();
        }
    }

    [RelayCommand]
    private void WatchAdForHint()
    {
        try
        {
            _feedbackService.LightTap();
            
            _adService.ShowRewardedAd(async () =>
            {
                try
                {
                    HintTokens++;
                    FeedbackMessage = AppResources.HintTokenEarned;
                    ShowFeedback = true;
                    await _feedbackService.PlaySuccessFeedback();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in ad reward callback: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing rewarded ad: {ex.Message}");
            FeedbackMessage = AppResources.UnableToShowAd;
            ShowFeedback = true;
            _ = _feedbackService.PlayErrorFeedback();
        }
    }

    [RelayCommand]
    private async Task ShareResult()
    {
        if (_currentPuzzle == null)
        {
            FeedbackMessage = AppResources.NoPuzzleToShare;
            ShowFeedback = true;
            await _feedbackService.PlayErrorFeedback();
            return;
        }

        try
        {
            _feedbackService.LightTap();
            
            // Get completion stats if available
            var stats = await _databaseService.GetCompletionStats(
                _currentPuzzle.DifficultySlot,
                DateTime.UtcNow.Date
            );
            
            // Build enhanced share text
            var difficultyName = _currentPuzzle.Difficulty switch
            {
                DifficultyLevel.Easy => "Easy",
                DifficultyLevel.Medium => "Medium",
                DifficultyLevel.Hard => "Hard",
                DifficultyLevel.Creative => "Creative",
                DifficultyLevel.Tricky => "Tricky",
                DifficultyLevel.Speed => "Speed",
                DifficultyLevel.Boss => "Boss",
                DifficultyLevel.Expert => "Expert",
                _ => "Unknown"
            };
            
            var shareText = $@"I solved PEMDAS puzzle {_currentPuzzle.PuzzleIdentifier}!
Difficulty: {difficultyName}
Time: {CompletionTimeDisplay}
Points: {stats?.PointsEarned ?? 0}
Total Points: {TotalPointsEarned}

Can you beat my time? Download PEMDAS now!";

            await Share.RequestAsync(new ShareTextRequest
            {
                Text = shareText,
                Title = "Share PEMDAS Result"
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error sharing result: {ex.Message}");
            FeedbackMessage = AppResources.UnableToShare;
            ShowFeedback = true;
            await _feedbackService.PlayErrorFeedback();
        }
    }

    [RelayCommand]
    private void AddDigit(string? digit)
    {
        if (!string.IsNullOrEmpty(digit))
        {
            if (ShowSingleInput)
            {
                UserInput += digit;
            }
            // For multi-value, numbers are typed directly into the Entry boxes
            // No need to append here as Entry fields handle their own input
            
            _feedbackService.LightTap();
        }
    }

    [RelayCommand]
    private void AddOperator(string? op)
    {
        if (!string.IsNullOrEmpty(op))
        {
            if (ShowSingleInput)
            {
                UserInput += $" {op} ";
            }
            // For multi-value, operators not needed (just numbers)
            
            _feedbackService.LightTap();
        }
    }

    [RelayCommand]
    private void AddParenthesis(string? paren)
    {
        if (!string.IsNullOrEmpty(paren))
        {
            if (ShowSingleInput)
            {
                UserInput += paren;
            }
            // For multi-value, parentheses not needed
            
            _feedbackService.LightTap();
        }
    }

    [RelayCommand]
    private void ClearInput()
    {
        UserInput = string.Empty;
        ValueA = string.Empty;
        ValueB = string.Empty;
        ValueC = string.Empty;
        _feedbackService.MediumImpact();
    }

    [RelayCommand]
    private void Backspace()
    {
        if (!string.IsNullOrEmpty(UserInput))
        {
            UserInput = UserInput[..^1];
            _feedbackService.LightTap();
        }
    }

    private async Task UpdateUserProgress()
    {
        try
        {
            var progress = await _databaseService.GetUserProgress();
            if (progress != null)
            {
                CurrentStreak = progress.CurrentStreak;
                HintTokens = progress.HintTokens;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating user progress: {ex.Message}");
        }
    }

    // Test mode support
    public void SetTestPuzzle(DailyPuzzle puzzle)
    {
        _testPuzzle = puzzle;
        IsTestMode = true;
    }

    public void ClearTestPuzzle()
    {
        _testPuzzle = null;
        IsTestMode = false;
        IsArchiveMode = false;
    }

    public void SetArchivePuzzle(DailyPuzzle puzzle)
    {
        _testPuzzle = puzzle;
        IsArchiveMode = true;
        IsTestMode = false;
    }

    [RelayCommand]
    private async Task ExitTestMode()
    {
        ClearTestPuzzle();
        await Shell.Current.GoToAsync("//testmode");
    }

    [RelayCommand]
    private async Task ExitArchiveMode()
    {
        ClearTestPuzzle();
        await Shell.Current.GoToAsync("archive");
    }
    
    // Difficulty selection and premium features
    [RelayCommand]
    private async Task SelectDifficulty(string slotString)
    {
        if (!int.TryParse(slotString, out var slot))
            return;
        
        try
        {
            System.Diagnostics.Debug.WriteLine($"🎯 SelectDifficulty called: slot {slot}");
            
            // Don't do anything if already on this difficulty
            if (slot == SelectedDifficultySlot)
                return;
            
            // Check if user can select Expert (requires subscription)
            if (slot == 7 && !IsSubscribed)
            {
                await ShowExpertLockedDialog();
                return;
            }
            
            // REMOVED: Old "unlock difficulties" ad requirement
            // With Wordle-style ads, users can select any difficulty
            // Ad will be shown in InitializeAsync when puzzle loads
            
            // Change difficulty
            SelectedDifficultySlot = slot;
            
            // Save preference
            var progress = await _databaseService.GetUserProgress();
            if (progress != null)
            {
                progress.PreferredDifficultySlot = slot;
                await _databaseService.UpdateUserProgress(progress);
            }
            
            // Reload puzzle with new difficulty (ad will be shown in InitializeAsync)
            await InitializeAsync();
            
            _feedbackService.LightTap();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error selecting difficulty: {ex.Message}");
            ErrorMessage = "Error changing difficulty. Please try again.";
            HasError = true;
        }
    }
    
    private async Task ShowExpertLockedDialog()
    {
        var result = await Application.Current.MainPage.DisplayActionSheet(
            "Expert Level - Premium Only",
            "Cancel",
            null,
            "Subscribe ($2.99/month)",
            "Learn More"
        );
        
        if (result == "Subscribe ($2.99/month)")
        {
            await NavigateToSubscription();
        }
        else if (result == "Learn More")
        {
            await Application.Current.MainPage.DisplayAlert(
                "Expert Level",
                "Expert level features advanced mathematical puzzles including:\n\n" +
                "• Exponential equations\n" +
                "• Logarithms\n" +
                "• Basic calculus\n" +
                "• Complex PEMDAS chains\n\n" +
                "Subscribe to unlock unlimited access to all difficulty levels!",
                "OK"
            );
        }
    }
    
    private async Task ShowUnlockDifficultyDialog()
    {
        var result = await Application.Current.MainPage.DisplayActionSheet(
            "Change Difficulty",
            "Cancel",
            null,
            "Watch Ad (Free)",
            "Subscribe ($2.99/month)"
        );
        
        if (result == "Watch Ad (Free)")
        {
            await WatchAdToUnlockDifficulty();
        }
        else if (result == "Subscribe ($2.99/month)")
        {
            await NavigateToSubscription();
        }
    }
    
    private async Task WatchAdToUnlockDifficulty()
    {
        try
        {
            _adService.ShowRewardedAd(async () =>
            {
                // Ad watched successfully
                HasWatchedAdToday = true;
                CanSelectDifficulty = true;
                
                // Update database
                var progress = await _databaseService.GetUserProgress();
                if (progress != null)
                {
                    progress.HasWatchedAdToday = true;
                    progress.LastAdWatchDate = DateTime.UtcNow;
                    await _databaseService.UpdateUserProgress(progress);
                }
                
                // Enable all difficulties except Expert
                UpdateDifficultyButtons();
                
                await Application.Current.MainPage.DisplayAlert(
                    "Unlocked!",
                    "You can now try any difficulty (except Expert) for the rest of today!",
                    "OK"
                );
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error showing ad: {ex.Message}");
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                "Unable to show ad. Please try again later.",
                "OK"
            );
        }
    }
    
    private async Task NavigateToSubscription()
    {
        try
        {
            // Navigate to subscription/premium page
            await Shell.Current.GoToAsync("//profile");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error navigating to subscription: {ex.Message}");
        }
    }
    
    private async Task UpdateSubscriptionStatus()
    {
        try
        {
            // Check subscription status (with null check)
            try
            {
                IsSubscribed = await _subscriptionService.CheckSubscriptionStatus();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking subscription: {ex.Message}");
                IsSubscribed = false; // Default to free user if check fails
            }
            
            // Get user progress for ad tracking
            var progress = await _databaseService.GetUserProgress();
            if (progress != null)
            {
                SelectedDifficultySlot = progress.PreferredDifficultySlot;
                
                var today = DateTime.UtcNow.Date;
                var lastAdDate = progress.LastAdWatchDate?.Date;
                
                // Reset ad watch status if it's a new day
                if (lastAdDate != today)
                {
                    HasWatchedAdToday = false;
                    progress.HasWatchedAdToday = false;
                    try
                    {
                        await _databaseService.UpdateUserProgress(progress);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error updating user progress: {ex.Message}");
                    }
                }
                else
                {
                    HasWatchedAdToday = progress.HasWatchedAdToday;
                }
            }
            else
            {
                // No user progress found, use defaults
                SelectedDifficultySlot = 0;
                HasWatchedAdToday = false;
            }
            
            // Determine if user can select difficulties
            if (IsSubscribed)
            {
                CanSelectDifficulty = true;
            }
            else
            {
                CanSelectDifficulty = HasWatchedAdToday;
            }
            
            // Update button states
            UpdateDifficultyButtons();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in UpdateSubscriptionStatus: {ex.Message}");
            
            // Default to free user if error
            IsSubscribed = false;
            CanSelectDifficulty = false;
            HasWatchedAdToday = false;
            SelectedDifficultySlot = 0;
            UpdateDifficultyButtons();
        }
    }
    
    private void UpdateDifficultyButtons()
    {
        // With Wordle-style ads, all difficulties are accessible
        // Ads will be shown when puzzle loads (in InitializeAsync)
        
        if (IsSubscribed)
        {
            // Subscribers: All difficulties enabled
            EasyEnabled = true;
            MediumEnabled = true;
            HardEnabled = true;
            CreativeEnabled = true;
            TrickyEnabled = true;
            SpeedEnabled = true;
            BossEnabled = true;
            ExpertEnabled = true;
        }
        else
        {
            // Free users: All except Expert enabled
            // (Wordle-style ad will show before each puzzle)
            EasyEnabled = true;
            MediumEnabled = true;
            HardEnabled = true;
            CreativeEnabled = true;
            TrickyEnabled = true;
            SpeedEnabled = true;
            BossEnabled = true;
            ExpertEnabled = false; // Expert requires subscription
        }
    }
}