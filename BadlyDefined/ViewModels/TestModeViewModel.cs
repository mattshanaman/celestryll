using BadlyDefined.Models;
using BadlyDefined.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace BadlyDefined.ViewModels;

public partial class TestModeViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly GameService _gameService;

    [ObservableProperty]
    private ObservableCollection<BadDefinition> allPuzzles = new();

    [ObservableProperty]
    private BadDefinition? selectedPuzzle;

    [ObservableProperty]
    private string searchQuery = "";

    [ObservableProperty]
    private int selectedDifficultyFilter = -1; // -1 = All

    [ObservableProperty]
    private int totalPuzzleCount;

    [ObservableProperty]
    private string libraryStats = "";

    [ObservableProperty]
    private string testGuess = "";

    [ObservableProperty]
    private string testResult = "";

    [ObservableProperty]
    private bool showTestResult;

    public TestModeViewModel(
        DatabaseService databaseService,
        GameService gameService)
    {
        _databaseService = databaseService;
        _gameService = gameService;
        Title = "Test Mode";
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;

        try
        {
            await LoadPuzzles();
            UpdateLibraryStats();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error initializing TestMode: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadPuzzles()
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var puzzles = new List<BadDefinition>();

            // Load 30 days of puzzles
            for (int i = 0; i < 30; i++)
            {
                var date = today.AddDays(i);
                var datePuzzles = await _databaseService.GetPuzzlesByDateAsync(date);
                puzzles.AddRange(datePuzzles);
            }

            // Apply filters
            if (SelectedDifficultyFilter >= 0)
            {
                puzzles = puzzles.Where(p => p.DifficultySlot == SelectedDifficultyFilter).ToList();
            }

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var query = SearchQuery.ToLowerInvariant();
                puzzles = puzzles.Where(p =>
                    p.Solution.ToLowerInvariant().Contains(query) ||
                    p.BadDefinitionText.ToLowerInvariant().Contains(query) ||
                    p.Category.ToLowerInvariant().Contains(query)
                ).ToList();
            }

            AllPuzzles = new ObservableCollection<BadDefinition>(puzzles.OrderBy(p => p.PuzzleDate).ThenBy(p => p.DifficultySlot));
            TotalPuzzleCount = puzzles.Count;

            Debug.WriteLine($"✅ Loaded {puzzles.Count} puzzles");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error loading puzzles: {ex.Message}");
        }
    }

    [RelayCommand]
    private void SelectPuzzle(BadDefinition puzzle)
    {
        SelectedPuzzle = puzzle;
        TestGuess = "";
        TestResult = "";
        ShowTestResult = false;
    }

    [RelayCommand]
    private void TestSolution()
    {
        if (SelectedPuzzle == null || string.IsNullOrWhiteSpace(TestGuess))
            return;

        _gameService.StartPuzzle(SelectedPuzzle);
        var result = _gameService.SubmitGuess(TestGuess);

        TestResult = result.IsCorrect
            ? $"✅ CORRECT!\nSolution: {SelectedPuzzle.Solution}\nPoints: {result.PointsEarned}"
            : $"❌ WRONG\nYour guess: {TestGuess}\nCorrect answer: {SelectedPuzzle.Solution}";

        ShowTestResult = true;
    }

    [RelayCommand]
    private async Task RegeneratePuzzles()
    {
        try
        {
            var confirm = await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                "Regenerate Puzzles",
                "This will delete all existing puzzles and recreate them. Continue?",
                "Yes",
                "Cancel");

            if (!confirm)
                return;

            IsBusy = true;

            await _databaseService.ClearAndRegeneratePuzzlesAsync();
            await LoadPuzzles();
            UpdateLibraryStats();

            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                "Success",
                "Puzzles regenerated successfully!",
                "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error regenerating puzzles: {ex.Message}");
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                "Error",
                "Failed to regenerate puzzles. Check logs.",
                "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void FilterByDifficulty(string slotString)
    {
        if (int.TryParse(slotString, out var slot))
        {
            SelectedDifficultyFilter = slot;
        }
        else
        {
            SelectedDifficultyFilter = -1; // All
        }

        _ = LoadPuzzles();
    }

    [RelayCommand]
    private void Search()
    {
        _ = LoadPuzzles();
    }

    private void UpdateLibraryStats()
    {
        var totalLibrary = Data.PuzzleLibrary.TotalPuzzles;
        var easy = Data.PuzzleLibrary.Easy.Puzzles.Count;
        var medium = Data.PuzzleLibrary.Medium.Puzzles.Count;
        var hard = Data.PuzzleLibrary.Hard.Puzzles.Count;

        LibraryStats = $"Library: {totalLibrary} puzzles\n" +
                      $"Easy: {easy} | Medium: {medium} | Hard: {hard}\n" +
                      $"Database: {TotalPuzzleCount} seeded";
    }
}
