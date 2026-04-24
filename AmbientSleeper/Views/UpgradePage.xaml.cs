namespace AmbientSleeper.Views;

public partial class UpgradePage : ContentPage
{
    public UpgradePage()
    {
        InitializeComponent();
    }

    private async void OnOpenPlans(object sender, EventArgs e)
    {
        // Navigate to your existing settings/subscription page if you have one
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private async void OnClose(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}