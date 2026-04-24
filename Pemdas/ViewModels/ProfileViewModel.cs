using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pemdas.Resources.Localization;
using Pemdas.Services;
using System.Text.RegularExpressions;

namespace Pemdas.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IFeedbackService _feedbackService;
    private readonly ErrorLoggingService _errorLogger;

    [ObservableProperty]
    private int currentStreak;

    [ObservableProperty]
    private int longestStreak;

    [ObservableProperty]
    private int totalPuzzlesSolved;

    [ObservableProperty]
    private int totalPoints;

    [ObservableProperty]
    private int hintTokens;

    [ObservableProperty]
    private bool isSubscribed;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private string userEmail = "";

    [ObservableProperty]
    private bool isEmailValid;

    [ObservableProperty]
    private string emailValidationMessage = "";

    public ProfileViewModel(
        DatabaseService databaseService, 
        ISubscriptionService subscriptionService,
        IFeedbackService feedbackService,
        ErrorLoggingService errorLogger)
    {
        _databaseService = databaseService ?? throw new ArgumentNullException(nameof(databaseService));
        _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
        _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
        _errorLogger = errorLogger ?? throw new ArgumentNullException(nameof(errorLogger));
        Title = AppResources.TabProfile;
    }

    partial void OnUserEmailChanged(string value)
    {
        ValidateEmail(value);
    }

    private void ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            IsEmailValid = false;
            EmailValidationMessage = "";
            return;
        }

        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        IsEmailValid = Regex.IsMatch(email, emailPattern);
        EmailValidationMessage = IsEmailValid ? "✓ Valid email" : "✗ Invalid email format";
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;
        HasError = false;

        try
        {
            var progress = await _databaseService.GetUserProgress();
            if (progress != null)
            {
                CurrentStreak = progress.CurrentStreak;
                LongestStreak = progress.LongestStreak;
                TotalPuzzlesSolved = progress.TotalPuzzlesSolved;
                TotalPoints = progress.TotalPoints;
                HintTokens = progress.HintTokens;
                IsSubscribed = await _subscriptionService.CheckSubscriptionStatus();
                UserEmail = progress.Email ?? "";

                // Celebrate milestones
                if (CurrentStreak > 0 && CurrentStreak % 7 == 0)
                {
                    await _feedbackService.PlayStreakFeedback();
                }
            }
            else
            {
                ErrorMessage = AppResources.FailedToLoadUserProgress;
                HasError = true;
                await _feedbackService.PlayErrorFeedback();
                System.Diagnostics.Debug.WriteLine("User progress is null");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error initializing profile: {ex.Message}");
            ErrorMessage = AppResources.ErrorLoadingProfile;
            HasError = true;
            await _feedbackService.PlayErrorFeedback();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveEmail()
    {
        if (!IsEmailValid && !string.IsNullOrWhiteSpace(UserEmail))
        {
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                AppResources.Error,
                "Please enter a valid email address.",
                AppResources.OK);
            return;
        }

        try
        {
            IsBusy = true;
            var progress = await _databaseService.GetUserProgress();
            if (progress != null)
            {
                progress.Email = string.IsNullOrWhiteSpace(UserEmail) ? null : UserEmail.Trim().ToLowerInvariant();
                progress.LastUpdated = DateTime.UtcNow;
                await _databaseService.UpdateUserProgress(progress);

                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                    AppResources.Success,
                    "Email saved successfully!",
                    AppResources.OK);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving email: {ex.Message}");
            await _errorLogger.LogErrorAsync(ex, "ProfileViewModel.SaveEmail", new Dictionary<string, object>
            {
                { "Email", UserEmail }
            });

            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                AppResources.Error,
                _errorLogger.GetUserFriendlyMessage(ex),
                AppResources.OK);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ShareStats()
    {
        try
        {
            var statsText = GenerateStatsShareText();

            await Share.RequestAsync(new ShareTextRequest
            {
                Text = statsText,
                Title = "Share My PEMDAS Stats"
            });

            System.Diagnostics.Debug.WriteLine("✅ Stats shared");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error sharing stats: {ex.Message}");
            await _errorLogger.LogErrorAsync(ex, "ProfileViewModel.ShareStats");

            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                AppResources.Error,
                "Unable to share stats. Please try again.",
                AppResources.OK);
        }
    }

    [RelayCommand]
    private async Task EmailStats()
    {
        if (string.IsNullOrWhiteSpace(UserEmail) || !IsEmailValid)
        {
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                AppResources.Error,
                "Please enter a valid email address first.",
                AppResources.OK);
            return;
        }

        try
        {
            var statsText = GenerateStatsShareText();
            var message = new EmailMessage
            {
                Subject = "My PEMDAS Daily Math Challenge Stats",
                Body = statsText,
                To = new List<string> { UserEmail }
            };

            await Email.ComposeAsync(message);
            System.Diagnostics.Debug.WriteLine("✅ Email composer opened");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Error emailing stats: {ex.Message}");
            await _errorLogger.LogErrorAsync(ex, "ProfileViewModel.EmailStats");

            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                AppResources.Error,
                "Unable to open email. Please ensure you have an email app configured.",
                AppResources.OK);
        }
    }

    private string GenerateStatsShareText()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("🎮 My PEMDAS Stats 🎮");
        sb.AppendLine();
        sb.AppendLine($"🔥 Current Streak: {CurrentStreak} days");
        sb.AppendLine($"⭐ Longest Streak: {LongestStreak} days");
        sb.AppendLine($"🎯 Total Puzzles Solved: {TotalPuzzlesSolved}");
        sb.AppendLine($"🏆 Total Points: {TotalPoints}");
        sb.AppendLine($"💡 Hint Tokens: {HintTokens}");
        sb.AppendLine();
        sb.AppendLine("Challenge your math skills with PEMDAS Daily Math Challenge! 🚀");

        return sb.ToString();
    }

    [RelayCommand]
    private async Task Subscribe()
    {
        IsBusy = true;
        HasError = false;
        _feedbackService.MediumImpact();

        try
        {
            var success = await _subscriptionService.PurchaseSubscription();
            if (success)
            {
                IsSubscribed = true;
                await _feedbackService.PlaySuccessFeedback();
                
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        AppResources.Success,
                        AppResources.SubscribeSuccess,
                        AppResources.OK);
                }
            }
            else
            {
                await _feedbackService.PlayErrorFeedback();
                
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        AppResources.Error,
                        AppResources.SubscriptionFailed,
                        AppResources.OK);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error subscribing: {ex.Message}");
            ErrorMessage = AppResources.ErrorDuringSubscription;
            HasError = true;
            await _feedbackService.PlayErrorFeedback();

            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    AppResources.Error,
                    AppResources.ErrorDuringSubscription,
                    AppResources.OK);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ViewArchive()
    {
        try
        {
            _feedbackService.LightTap();

            IsSubscribed = await _subscriptionService.CheckSubscriptionStatus();
            
            if (!IsSubscribed)
            {
                await _feedbackService.PlayErrorFeedback();
                
                if (Application.Current?.MainPage != null)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        AppResources.SubscribeRequired,
                        AppResources.SubscribeRequired,
                        AppResources.OK);
                }
                return;
            }

            await Shell.Current.GoToAsync("archive");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error viewing archive: {ex.Message}");
            await _feedbackService.PlayErrorFeedback();
            
            if (Application.Current?.MainPage != null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    AppResources.Error,
                    AppResources.UnableToAccessArchive,
                    AppResources.OK);
            }
        }
    }
}