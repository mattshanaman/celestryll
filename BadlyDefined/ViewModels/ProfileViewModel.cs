using BadlyDefined.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace BadlyDefined.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly DatabaseService _databaseService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly ErrorLoggingService _errorLogger;

    [ObservableProperty]
    private int currentStreak;

    [ObservableProperty]
    private int longestStreak;

    [ObservableProperty]
    private int totalPuzzlesCompleted;

    [ObservableProperty]
    private int totalPoints;

    [ObservableProperty]
    private int hintTokens;

    [ObservableProperty]
    private int easyCompleted;

    [ObservableProperty]
    private int mediumCompleted;

    [ObservableProperty]
    private int hardCompleted;

    [ObservableProperty]
    private double averageAttempts;

    [ObservableProperty]
    private int bestAttempts;

    [ObservableProperty]
    private bool isSubscribed;

    [ObservableProperty]
    private string subscriptionStatus = "Free";

    [ObservableProperty]
    private string userEmail = "";

    [ObservableProperty]
    private bool isEmailValid;

    [ObservableProperty]
    private string emailValidationMessage = "";

    public ProfileViewModel(
        DatabaseService databaseService,
        ISubscriptionService subscriptionService,
        ErrorLoggingService errorLogger)
    {
        _databaseService = databaseService;
        _subscriptionService = subscriptionService;
        _errorLogger = errorLogger;
        Title = "Profile";
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

        // Basic email validation
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        IsEmailValid = Regex.IsMatch(email, emailPattern);
        EmailValidationMessage = IsEmailValid ? "✓ Valid email" : "✗ Invalid email format";
    }

    public async Task InitializeAsync()
    {
        IsBusy = true;

        try
        {
            var progress = await _databaseService.GetUserProgressAsync();

            CurrentStreak = progress.CurrentStreak;
            LongestStreak = progress.LongestStreak;
            TotalPuzzlesCompleted = progress.TotalPuzzlesCompleted;
            TotalPoints = progress.TotalPoints;
            HintTokens = progress.HintTokens;
            EasyCompleted = progress.EasyCompleted;
            MediumCompleted = progress.MediumCompleted;
            HardCompleted = progress.HardCompleted;
            AverageAttempts = Math.Round(progress.AverageAttempts, 1);
            BestAttempts = progress.BestAttempts == int.MaxValue ? 0 : progress.BestAttempts;
            UserEmail = progress.Email ?? "";

            IsSubscribed = await _subscriptionService.CheckSubscriptionStatus();
            SubscriptionStatus = IsSubscribed ? "Premium ⭐" : "Free";

            Debug.WriteLine("✅ Profile loaded");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error loading profile: {ex.Message}");
            await _errorLogger.LogErrorAsync(ex, "ProfileViewModel.InitializeAsync");
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
                "Invalid Email",
                "Please enter a valid email address.",
                "OK");
            return;
        }

        try
        {
            IsBusy = true;
            var progress = await _databaseService.GetUserProgressAsync();
            progress.Email = string.IsNullOrWhiteSpace(UserEmail) ? null : UserEmail.Trim().ToLowerInvariant();
            progress.LastUpdated = DateTime.UtcNow;
            await _databaseService.UpdateUserProgressAsync(progress);

            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                "Success",
                "Email saved successfully!",
                "OK");

            Debug.WriteLine($"✅ Email saved: {progress.Email}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error saving email: {ex.Message}");
            await _errorLogger.LogErrorAsync(ex, "ProfileViewModel.SaveEmail", new Dictionary<string, object>
            {
                { "Email", UserEmail }
            });

            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                "Error",
                _errorLogger.GetUserFriendlyMessage(ex),
                "OK");
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
                Title = "Share My BadlyDefined Stats"
            });

            Debug.WriteLine("✅ Stats shared");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error sharing stats: {ex.Message}");
            await _errorLogger.LogErrorAsync(ex, "ProfileViewModel.ShareStats");

            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                "Error",
                "Unable to share stats. Please try again.",
                "OK");
        }
    }

    [RelayCommand]
    private async Task EmailStats()
    {
        if (string.IsNullOrWhiteSpace(UserEmail) || !IsEmailValid)
        {
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                "Email Required",
                "Please enter a valid email address first.",
                "OK");
            return;
        }

        try
        {
            var statsText = GenerateStatsShareText();
            var message = new EmailMessage
            {
                Subject = "My BadlyDefined Stats",
                Body = statsText,
                To = new List<string> { UserEmail }
            };

            await Email.ComposeAsync(message);
            Debug.WriteLine("✅ Email composer opened");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error emailing stats: {ex.Message}");
            await _errorLogger.LogErrorAsync(ex, "ProfileViewModel.EmailStats");

            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                "Error",
                "Unable to open email. Please ensure you have an email app configured.",
                "OK");
        }
    }

    private string GenerateStatsShareText()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("🎮 My BadlyDefined Stats 🎮");
        sb.AppendLine();
        sb.AppendLine($"🔥 Current Streak: {CurrentStreak} days");
        sb.AppendLine($"⭐ Longest Streak: {LongestStreak} days");
        sb.AppendLine($"🎯 Total Puzzles Solved: {TotalPuzzlesCompleted}");
        sb.AppendLine($"🏆 Total Points: {TotalPoints}");
        sb.AppendLine($"💡 Hint Tokens: {HintTokens}");
        sb.AppendLine();
        sb.AppendLine("By Difficulty:");
        sb.AppendLine($"⭐ Easy: {EasyCompleted}");
        sb.AppendLine($"⭐⭐ Medium: {MediumCompleted}");
        sb.AppendLine($"⭐⭐⭐ Hard: {HardCompleted}");
        sb.AppendLine();
        sb.AppendLine($"📊 Average Attempts: {AverageAttempts}");
        if (BestAttempts > 0)
        {
            sb.AppendLine($"🎖️ Best Attempts: {BestAttempts}");
        }
        sb.AppendLine();
        sb.AppendLine("Play BadlyDefined and challenge your friends! 🚀");

        return sb.ToString();
    }

    [RelayCommand]
    private async Task Subscribe()
    {
        try
        {
            var result = await _subscriptionService.PurchaseSubscription("badlydefined.premium.monthly");
            if (result)
            {
                IsSubscribed = true;
                SubscriptionStatus = "Premium ⭐";
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                    "Success!",
                    "Thank you for subscribing to BadlyDefined Premium!",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error subscribing: {ex.Message}");
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                "Error",
                "Unable to process subscription. Please try again.",
                "OK");
        }
    }

    [RelayCommand]
    private async Task RestorePurchases()
    {
        try
        {
            var result = await _subscriptionService.RestorePurchases();
            if (result)
            {
                await Application.Current!.Windows[0].Page!.DisplayAlertAsync(
                    "Restored",
                    "Your purchases have been restored!",
                    "OK");
                await InitializeAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"❌ Error restoring: {ex.Message}");
        }
    }
}
