using AmbientSleeper.Views;

namespace AmbientSleeper
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for pages opened via Shell navigation
            Routing.RegisterRoute(nameof(HelpPage), typeof(HelpPage));
            Routing.RegisterRoute(nameof(LegalPage), typeof(LegalPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
            Routing.RegisterRoute(nameof(PlaybackSettingsPage), typeof(PlaybackSettingsPage));
            Routing.RegisterRoute(nameof(EqPage), typeof(EqPage));
            Routing.RegisterRoute(nameof(UpgradePage), typeof(UpgradePage));
        }

        private async void OnHelpClicked(object sender, EventArgs e)
        {
            // Close the flyout
            FlyoutIsPresented = false;

            // Navigate to Help page
            await Shell.Current.GoToAsync(nameof(HelpPage));
        }

        private async void OnLegalClicked(object sender, EventArgs e)
        {
            // Close the flyout
            FlyoutIsPresented = false;

            // Navigate to Legal page
            await Shell.Current.GoToAsync(nameof(LegalPage));
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            // Close the flyout
            FlyoutIsPresented = false;

            // Navigate to Settings page
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }
    }
}
