using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pemdas.Models;
using Pemdas.Services;
using System.Collections.ObjectModel;

namespace Pemdas.ViewModels;

public partial class PastGamesViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly GameViewModel _gameViewModel;
    private readonly IFeedbackService _feedbackService;

    [ObservableProperty]
    private ObservableCollection<PastGameItem> pastGames = new();

    [ObservableProperty]
    private bool isSubscribed;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    public PastGamesViewModel(
        DatabaseService databaseService,
        ISubscriptionService subscriptionService,
        GameViewModel gameViewModel,
        IFeedbackService feedbackService)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
        _gameViewModel = gameViewModel ?? throw new ArgumentNullException(nameof(gameViewModel));
        _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
        Title = "Past Games";
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        HasError = false;
        ErrorMessage = string.Empty;

        try
        {
            IsSubscribed = await _subscriptionService.CheckSubscriptionStatus();
            if (!IsSubscribed)
            {
                PastGames.Clear();
                return;
            }

            var archivePuzzles = await _databaseService.GetPuzzleArchive(page: 0, pageSize: 120);

            PastGames = new ObservableCollection<PastGameItem>(
                archivePuzzles
                    .OrderByDescending(p => p.PuzzleDate)
                    .ThenBy(p => p.DifficultySlot)
                    .Select(p => new PastGameItem
                    {
                        Puzzle = p,
                        PuzzleDate = p.PuzzleDate,
                        PuzzleId = p.PuzzleIdentifier,
                        Difficulty = p.Difficulty.ToString(),
                        Mode = p.Mode == PuzzleMode.SolveIt ? "Solve It" : "Build It"
                    }));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading past games: {ex.Message}");
            HasError = true;
            ErrorMessage = "Unable to load past games right now.";
            await _feedbackService.PlayErrorFeedback();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task OpenPastGame(PastGameItem? item)
    {
        if (item?.Puzzle == null)
            return;

        var hasAccess = await _subscriptionService.CheckSubscriptionStatus();
        if (!hasAccess)
        {
            await _feedbackService.PlayErrorFeedback();
            return;
        }

        _feedbackService.LightTap();
        _gameViewModel.SetArchivePuzzle(item.Puzzle);
        await Shell.Current.GoToAsync("//game");
    }
}

public class PastGameItem
{
    public DailyPuzzle Puzzle { get; set; } = new();
    public DateTime PuzzleDate { get; set; }
    public string PuzzleId { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;

    public string DateDisplay => PuzzleDate.ToString("yyyy-MM-dd");
    public string Subtitle => $"{Difficulty} • {Mode}";
}
