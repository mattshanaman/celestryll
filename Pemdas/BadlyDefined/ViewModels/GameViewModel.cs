using BadlyDefined.Models;
using BadlyDefined.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BadlyDefined.ViewModels;

public partial class GameViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly GameService _gameService;
    private readonly IAdService _adService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IFeedbackService _feedbackService;
    private readonly ErrorLoggingService _errorLogger;

    private BadDefinition? _currentPuzzle;
    private DateTime _puzzleStartTime;
    private IDispatcherTimer? _elapsedTimer;

    // ==================== OBSERVABLE PROPERTIES ====================

    [ObservableProperty]
    private string puzzleDefinition = "";

    [ObservableProperty]
    private string categoryHint = "";

    [ObservableProperty]
    private string revealedWord = "";

    [ObservableProperty]
    private int letterCount;

    [ObservableProperty]
    private string userGuess = "";

    [ObservableProperty]
    private ObservableCollection<GuessAttempt> previousGuesses = new();

    [ObservableProperty]
    private int attemptsCount;

    [ObservableProperty]
    private int maxAttempts = 5;

    [ObservableProperty]
    private int currentPoints;

    [ObservableProperty]
    private int currentHintLevel;

    [ObservableProperty]
    private bool puzzleCompleted;

    [ObservableProperty]
    private int currentStreak;

    [ObservableProperty]
    private int hintTokens;

    [ObservableProperty]
    private string elapsedTime = "0:00";

    [ObservableProperty]
    private string currentPuzzleId = "";

    [ObservableProperty]
    private int selectedDifficultySlot;

    [ObservableProperty]
    private bool easyEnabled = true;

    [ObservableProperty]
    private bool mediumEnabled = true;

    [ObservableProperty]
    private bool hardEnabled = true;

    [ObservableProperty]
    private bool isSubscribed;

    [ObservableProperty]
    private string feedbackMessage = "";

    [ObservableProperty]
    private bool showFeedback;

    [ObservableProperty]
    private string errorMessage = "";

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private int totalPointsEarned;

    [ObservableProperty]
    private bool hasCompletedThisDifficulty;

    [ObservableProperty]
    private string completionMessage = "";

    [ObservableProperty]
    private ObservableCollection<int> completedDifficulties = new();

    [ObservableProperty]
    private string completedDifficultiesDisplay = "";

    public string CompletionTimeDisplay => 
        _currentPuzzle != null && PuzzleCompleted 
            ? $"{(int)(DateTime.UtcNow - _puzzleStartTime).TotalMinutes}:{(int)(DateTime.UtcNow - _puzzleStartTime).TotalSeconds % 60:D2}"
            : "0:00";

    public GameViewModel(
        DatabaseService databaseService,
        GameService gameService,
        IAdService adService,
        ISubscriptionService subscriptionService,
        IFeedbackService feedbackService,
        ErrorLoggingService errorLogger)
    {
        _databaseService = databaseService;
        _gameService = gameService;
        _adService = adService;
        _subscriptionService = subscriptionService;
        _feedbackService = feedbackService;
        _errorLogger = errorLogger;

        Title = "Badly Defined";
    }

    // ==================== INITIALIZATION ====================

    public async Task InitializeAsync()
    {
        IsBusy = true;
        HasError = false;
        ShowFeedback = false;

        try
        {
            Debug.WriteLine("🚀 GameViewModel.InitializeAsync started");

            // Ensure database is initialized first
            try
            {
                Debug.WriteLine("🔧 Ensuring database is initialized...");
                await _databaseService.InitializeAsync();
                Debug.WriteLine("✅ Database initialization confirmed");
            }
            catch (Exception dbEx)
            {
                Debug.WriteLine($"❌ Database initialization error: {dbEx.Message}");
                Debug.WriteLine($"❌ Stack: {dbEx.StackTrace}");
                ErrorMessage = $"Database error. Please restart the app.";
                HasError = true;
                IsBusy = false;

                // Don't throw - let the app show the error instead of crashing
                return;
            }

            // Update subscription status
            try
            {
                IsSubscribed = await _subscriptionService.CheckSubscriptionStatus();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking subscription: {ex.Message}");
                IsSubscribed = false;
            }

            // Wordle-style ad (before puzzle loads)
            if (!IsSubscribed)
            {
                try
                {
                    Debug.WriteLine("📺 Showing Wordle-style ad...");
                    _adService.ShowInterstitialAd();
                    await Task.Delay(500);
                }
                catch (Exception adEx)
                {
                    Debug.WriteLine($"⚠️ Error showing ad: {adEx.Message}");
                }
            }

            // Load puzzle
            Debug.WriteLine($"🔍 Loading puzzle for difficulty slot {SelectedDifficultySlot}...");
            var (puzzle, alreadyPlayed) = await _gameService.GetTodaysPuzzleAsync(SelectedDifficultySlot);

            if (puzzle == null)
            {
                Debug.WriteLine($"⚠️ No puzzle returned from GameService");
                ErrorMessage = "Unable to load puzzle. Please try again.";
                HasError = true;
                IsBusy = false;
                return;
            }

            _currentPuzzle = puzzle;
            _gameService.StartPuzzle(puzzle);

            // Setup UI
            PuzzleDefinition = puzzle.BadDefinitionText;
            LetterCount = puzzle.LetterCount;
            MaxAttempts = puzzle.MaxAttempts;
            CurrentPoints = puzzle.BasePoints;
            CurrentPuzzleId = puzzle.PuzzleIdentifier;
            RevealedWord = _gameService.GetRevealedWord();
            CategoryHint = "Make a guess to reveal the category";

            // Check if already completed
            HasCompletedThisDifficulty = alreadyPlayed;
            PuzzleCompleted = alreadyPlayed;

            // Load user progress
            await UpdateUserProgress();

            // Load completion tracking
            await LoadCompletionTracking();

            // Start elapsed timer only if not completed
            if (!PuzzleCompleted)
            {
                StartElapsedTimer();
            }

            Debug.WriteLine($"✅ Puzzle loaded: {puzzle.Solution}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error initializing: {ex.Message}");
            ErrorMessage = "Error loading puzzle. Please restart the app.";
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ==================== GUESS SUBMISSION ====================

    [RelayCommand]
    private async Task SubmitGuess()
    {
        if (string.IsNullOrWhiteSpace(UserGuess))
            return;

        if (PuzzleCompleted)
            return;

        IsBusy = true;
        HasError = false;
        ShowFeedback = false;

        try
        {
            var result = _gameService.SubmitGuess(UserGuess);

            AttemptsCount = result.AttemptsUsed;
            CurrentPoints = Math.Max(10, CurrentPoints - (_currentPuzzle!.BasePoints / 5));

            if (result.IsCorrect)
            {
                // Correct!
                StopElapsedTimer();
                PuzzleCompleted = true;
                FeedbackMessage = $"✅ Correct! +{result.PointsEarned} points!";
                ShowFeedback = true;

                await _feedbackService.PlaySuccessFeedback();

                // Record completion
                await _gameService.CompletePuzzleAsync();

                // Update tracking
                await LoadCompletionTracking();

                // Update progress
                await UpdateUserProgress();

                // Stop timer if all puzzles completed
                if (CompletedDifficulties.Count == 3)
                {
                    StopElapsedTimer();
                }
            }
            else
            {
                // Wrong guess
                RevealedWord = result.RevealedWord;
                FeedbackMessage = result.Message;
                ShowFeedback = true;

                await _feedbackService.PlayErrorFeedback();

                // After first guess, reveal category and one letter
                if (AttemptsCount == 1)
                {
                    CategoryHint = $"Category: {_currentPuzzle!.Category}";
                    _gameService.RevealNextHint();
                    RevealedWord = _gameService.GetRevealedWord();
                }
                else
                {
                    // Update hint level
                    CurrentHintLevel = _gameService.GetCurrentState().CurrentHintLevel;
                    CategoryHint = _gameService.GetHintText();
                }
            }

            // Add to previous guesses
            PreviousGuesses.Insert(0, new GuessAttempt
            {
                GuessText = UserGuess,
                IsCorrect = result.IsCorrect,
                AttemptNumber = result.AttemptsUsed,
                Timestamp = DateTime.UtcNow
            });

            // Clear input
            UserGuess = "";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error submitting guess: {ex.Message}");
            ErrorMessage = "Error processing guess. Please try again.";
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ==================== HINT SYSTEM ====================

    [RelayCommand]
    private async Task UseHint()
    {
        if (PuzzleCompleted)
            return;

        try
        {
            var progress = await _databaseService.GetUserProgressAsync();
            
            if (progress.HintTokens <= 0)
            {
                ErrorMessage = "No hint tokens available. Complete puzzles to earn more!";
                HasError = true;
                return;
            }

            // Use hint token
            progress.HintTokens--;
            await _databaseService.UpdateUserProgressAsync(progress);
            HintTokens = progress.HintTokens;

            // Reveal next hint
            _gameService.RevealNextHint();
            RevealedWord = _gameService.GetRevealedWord();
            CategoryHint = _gameService.GetHintText();
            CurrentHintLevel = _gameService.GetCurrentState().CurrentHintLevel;

            FeedbackMessage = "💡 Hint revealed!";
            ShowFeedback = true;

            await _feedbackService.LightTap();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error using hint: {ex.Message}");
            ErrorMessage = "Error using hint. Please try again.";
            HasError = true;
        }
    }

    // ==================== DIFFICULTY SELECTION ====================

    [RelayCommand]
    private async Task SelectDifficulty(string slotString)
    {
        if (!int.TryParse(slotString, out var slot))
            return;

        if (slot == SelectedDifficultySlot)
            return;

        try
        {
            SelectedDifficultySlot = slot;

            // Save preference
            var progress = await _databaseService.GetUserProgressAsync();
            progress.PreferredDifficultySlot = slot;
            await _databaseService.UpdateUserProgressAsync(progress);

            // Reload puzzle
            await InitializeAsync();

            await _feedbackService.LightTap();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error selecting difficulty: {ex.Message}");
            ErrorMessage = "Error changing difficulty. Please try again.";
            HasError = true;
        }
    }

    // ==================== SHARING ====================

    [RelayCommand]
    private async Task ShareResult()
    {
        if (_currentPuzzle == null || !PuzzleCompleted)
            return;

        try
        {
            var completionTime = (int)(DateTime.UtcNow - _puzzleStartTime).TotalSeconds;
            var shareText = _gameService.GenerateShareableResult(
                _currentPuzzle,
                AttemptsCount,
                CurrentPoints,
                completionTime
            );

            await Share.RequestAsync(new ShareTextRequest
            {
                Text = shareText,
                Title = "Share BadlyDefined Result"
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error sharing: {ex.Message}");
        }
    }

    // ==================== HELPERS ====================

    private async Task UpdateUserProgress()
    {
        try
        {
            var progress = await _databaseService.GetUserProgressAsync();
            CurrentStreak = progress.CurrentStreak;
            HintTokens = progress.HintTokens;
            TotalPointsEarned = progress.TotalPoints;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error updating user progress: {ex.Message}");
        }
    }

    private async Task LoadCompletionTracking()
    {
        try
        {
            var completed = await _databaseService.GetTodaysCompletedDifficultiesAsync();
            CompletedDifficulties = new ObservableCollection<int>(completed);

            HasCompletedThisDifficulty = completed.Contains(SelectedDifficultySlot);

            UpdateCompletedDifficultiesDisplay();
            UpdateCompletionMessage();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error loading completion tracking: {ex.Message}");
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
            icons.Add(slot switch
            {
                0 => "⭐",
                1 => "⭐⭐",
                2 => "⭐⭐⭐",
                _ => "?"
            });
        }

        CompletedDifficultiesDisplay = string.Join(" ", icons);
    }

    private void UpdateCompletionMessage()
    {
        if (HasCompletedThisDifficulty)
        {
            var remaining = 3 - CompletedDifficulties.Count;
            CompletionMessage = remaining > 0
                ? $"Try the remaining {remaining} difficulty level{(remaining > 1 ? "s" : "")}!"
                : "Amazing! You've completed all 3 difficulties today! 🎉";
        }
        else
        {
            CompletionMessage = "";
        }
    }

    private void StartElapsedTimer()
    {
        _puzzleStartTime = DateTime.UtcNow;
        _elapsedTimer = Application.Current!.Dispatcher.CreateTimer();
        _elapsedTimer.Interval = TimeSpan.FromSeconds(1);
        _elapsedTimer.Tick += (s, e) =>
        {
            var elapsed = DateTime.UtcNow - _puzzleStartTime;
            ElapsedTime = $"{(int)elapsed.TotalMinutes}:{elapsed.Seconds:D2}";
        };
        _elapsedTimer.Start();
    }

    private void StopElapsedTimer()
    {
        _elapsedTimer?.Stop();
    }
}
