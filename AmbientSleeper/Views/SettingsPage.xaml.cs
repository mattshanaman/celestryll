using AmbientSleeper.Models;
using AmbientSleeper.Services;
using Microsoft.Maui.ApplicationModel;
using AmbientSleeper.Resources.Strings;

namespace AmbientSleeper.Views;

public partial class SettingsPage : ContentPage
{
    private readonly ISubscriptionService? _subscription;
    private readonly IErrorReportingService? _errorReporting;
    private readonly IUserNotificationService? _notificationService;
    private bool _isUpdatingUi = false; // NEW: Flag to prevent recursive UI updates

    public SettingsPage()
    {
        try
        {
            InitializeComponent();
            var sp = ServiceHost.Services;
            
            _subscription = (sp != null)
                ? sp.GetRequiredService<ISubscriptionService>()
                : null;

            _errorReporting = sp?.GetService<IErrorReportingService>();
            _notificationService = sp?.GetService<IUserNotificationService>();

            if (_subscription == null)
            {
                // Handle gracefully - maybe show error message
                return;
            }

            // reflect current plan
            ApplySelectionFromState();
            UpdateCurrentTierLabel();

            // keep UI synced if plan changes externally
            // (e.g., from a restore purchase flow)
            _subscription.TierChanged += (_, __) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (!_isUpdatingUi) // NEW: Only update if not already updating
                    {
                        ApplySelectionFromState();
                        UpdateCurrentTierLabel();
                    }
                });
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }

    private void UpdateCurrentTierLabel()
    {
        if (_subscription == null) return;
        
        var lifetime = _subscription.IsLifetime ? AppResources.LifetimeSuffix : string.Empty;
        CurrentTierLabel.Text = string.Format(AppResources.CurrentTier_Format, _subscription.CurrentTier, lifetime);
    }

    // Select the appropriate radio and enable/disable recurring radios
    private void ApplySelectionFromState()
    {
        if (_subscription == null) return;
        
        _isUpdatingUi = true; // NEW: Set flag to prevent recursion
        try
        {
            // Clear first to avoid two-checked UI if GroupName is reused
            FreeRadio.IsChecked = false;
            StandardSubRadio.IsChecked = false;
            PremiumSubRadio.IsChecked = false;
            ProPlusSubRadio.IsChecked = false;
            StandardLifetimeRadio.IsChecked = false;
            PremiumLifetimeRadio.IsChecked = false;

            // Enable/disable recurring options when lifetime is active
            var recurringEnabled = !_subscription.IsLifetime;
            FreeRadio.IsEnabled = recurringEnabled;
            StandardSubRadio.IsEnabled = recurringEnabled;
            PremiumSubRadio.IsEnabled = recurringEnabled;
            ProPlusSubRadio.IsEnabled = recurringEnabled;

            // Set the selected radio
            if (_subscription.IsLifetime)
            {
                // Lifetime: choose the corresponding lifetime radio
                switch (_subscription.CurrentTier)
                {
                    case SubscriptionTier.Standard:
                        StandardLifetimeRadio.IsChecked = true;
                        break;
                    case SubscriptionTier.Premium:
                        PremiumLifetimeRadio.IsChecked = true;
                        break;
                    default:
                        // If lifetime somehow set for Pro+/Free, fall back to displaying tier text only
                        // and keep all radios unchecked.
                        break;
                }
            }
            else
            {
                switch (_subscription.CurrentTier)
                {
                    case SubscriptionTier.Free:
                        FreeRadio.IsChecked = true;
                        break;
                    case SubscriptionTier.Standard:
                        StandardSubRadio.IsChecked = true;
                        break;
                    case SubscriptionTier.Premium:
                        PremiumSubRadio.IsChecked = true;
                        break;
                    case SubscriptionTier.ProPlus:
                        ProPlusSubRadio.IsChecked = true;
                        break;
                }
            }
        }
        finally
        {
            _isUpdatingUi = false; // NEW: Reset flag when done
        }
    }

    // Recurring selections
    private void OnFreeChecked(object? sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || _isUpdatingUi || _subscription == null) return; // NEW: Skip if this is a UI update cycle or no subscription service
        _subscription.SetTier(SubscriptionTier.Free);
        ApplySelectionFromState();
        UpdateCurrentTierLabel();
    }

    private void OnStandardChecked(object? sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || _isUpdatingUi || _subscription == null) return; // NEW: Skip if this is a UI update cycle or no subscription service
        _subscription.SetTier(SubscriptionTier.Standard);
        ApplySelectionFromState();
        UpdateCurrentTierLabel();
    }

    private void OnPremiumChecked(object? sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || _isUpdatingUi || _subscription == null) return; // NEW: Skip if this is a UI update cycle or no subscription service
        _subscription.SetTier(SubscriptionTier.Premium);
        ApplySelectionFromState();
        UpdateCurrentTierLabel();
    }

    private void OnProPlusChecked(object? sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || _isUpdatingUi || _subscription == null) return; // NEW: Skip if this is a UI update cycle or no subscription service
        _subscription.SetTier(SubscriptionTier.ProPlus);
        ApplySelectionFromState();
        UpdateCurrentTierLabel();
    }

    // Lifetime selections
    private void OnStandardLifetimeChecked(object? sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || _isUpdatingUi || _subscription == null) return; // NEW: Skip if this is a UI update cycle or no subscription service
        _subscription.SetLifetimeTier(SubscriptionTier.Standard);
        ApplySelectionFromState();
        DisplayToast(AppResources.Purchased_StandardLifetime);
    }

    private void OnPremiumLifetimeChecked(object? sender, CheckedChangedEventArgs e)
    {
        if (!e.Value || _isUpdatingUi || _subscription == null) return; // NEW: Skip if this is a UI update cycle or no subscription service
        _subscription.SetLifetimeTier(SubscriptionTier.Premium);
        ApplySelectionFromState();
        DisplayToast(AppResources.Purchased_PremiumLifetime);
    }

    private async void DisplayToast(string message)
    {
        await DisplayAlert(AppResources.Subscription_Title, message, AppResources.Ok);
        UpdateCurrentTierLabel();
    }

    // Diagnostics handlers
    private async void OnCheckHealthClicked(object? sender, EventArgs e)
    {
        try
        {
            HealthStatusLabel.Text = AppResources.HealthCheck_Checking;
            
            var (isHealthy, issues) = UserPreferences.CheckHealth();
            
            if (isHealthy)
            {
                HealthStatusLabel.Text = AppResources.HealthCheck_AllHealthy;
                HealthStatusLabel.TextColor = Colors.Green;
                
                if (_notificationService != null)
                {
                    await _notificationService.ShowToastAsync(AppResources.HealthCheck_Passed, NotificationType.Success);
                }
            }
            else
            {
                HealthStatusLabel.Text = string.Format(AppResources.HealthCheck_IssuesDetected, issues.Count);
                HealthStatusLabel.TextColor = Colors.Orange;
                
                var issueText = string.Join("\n• ", issues);
                await DisplayAlert(AppResources.HealthCheck_ResultsTitle, 
                    string.Format(AppResources.HealthCheck_IssuesMessage, issueText), 
                    AppResources.Ok);
            }
        }
        catch (Exception ex)
        {
            HealthStatusLabel.Text = AppResources.HealthCheck_Failed;
            HealthStatusLabel.TextColor = Colors.Red;
            
            await DisplayAlert(AppResources.Error, 
                string.Format(AppResources.HealthCheck_FailedMessage, ex.Message), 
                AppResources.Ok);
        }
    }

    private async void OnViewErrorReportClicked(object? sender, EventArgs e)
    {
        try
        {
            if (_errorReporting == null)
            {
                await DisplayAlert(AppResources.Error, 
                    AppResources.ErrorReport_ServiceUnavailable, 
                    AppResources.Ok);
                return;
            }

            var errors = _errorReporting.GetErrors();
            
            if (errors.Count == 0)
            {
                await DisplayAlert(AppResources.ErrorReport_Title, 
                    AppResources.ErrorReport_NoErrors, 
                    AppResources.Ok);
                return;
            }

            var choice = await DisplayActionSheet(
                string.Format(AppResources.ErrorReport_CountFormat, errors.Count),
                AppResources.Cancel,
                null,
                AppResources.ErrorReport_ViewDetails,
                AppResources.ErrorReport_ShareReport,
                AppResources.ErrorReport_ClearErrors);

            switch (choice)
            {
                case var c when c == AppResources.ErrorReport_ViewDetails:
                    var report = await _errorReporting.GenerateReportAsync();
                    await DisplayAlert(AppResources.ErrorReport_Title, report, AppResources.Ok);
                    break;

                case var c when c == AppResources.ErrorReport_ShareReport:
                    await _errorReporting.ShareReportAsync();
                    if (_notificationService != null)
                    {
                        await _notificationService.ShowToastAsync(AppResources.ErrorReport_SharedSuccess, NotificationType.Success);
                    }
                    break;

                case var c when c == AppResources.ErrorReport_ClearErrors:
                    var confirm = await DisplayAlert(AppResources.ErrorReport_ClearConfirmTitle, 
                        AppResources.ErrorReport_ClearConfirmMessage, 
                        AppResources.Yes, 
                        AppResources.No);
                    
                    if (confirm)
                    {
                        _errorReporting.ClearErrors();
                        if (_notificationService != null)
                        {
                            await _notificationService.ShowToastAsync(AppResources.ErrorReport_Cleared, NotificationType.Info);
                        }
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert(AppResources.Error, 
                string.Format(AppResources.ErrorReport_ViewFailed, ex.Message), 
                AppResources.Ok);
        }
    }
}