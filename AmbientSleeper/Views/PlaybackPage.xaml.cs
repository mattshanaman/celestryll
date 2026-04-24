using AmbientSleeper.Models;
using AmbientSleeper.Services;
using AmbientSleeper.ViewModels;
using AmbientSleeper.Resources.Strings;
using static AmbientSleeper.ViewModels.PlaybackViewModel;

namespace AmbientSleeper.Views;

public partial class PlaybackPage : ContentPage
{
    public PlaybackPage(PlaybackViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Refresh fade-out text from persisted preference
        if (BindingContext is PlaybackViewModel vm)
        {
            vm.FadeOutSeconds = UserPreferences.FadeOutSeconds;
        }
    }

    private void OnMixVolumeChanged(object sender, ValueChangedEventArgs e)
    {
        if (sender is Slider slider && slider.BindingContext is AudioItem item)
        {
            var vm = BindingContext as PlaybackViewModel;
            vm?.UpdateMixVolumeCommand.Execute(new MixVolumeUpdate { Item = item, Volume = e.NewValue });
        }
    }

    private async void OnOpenSettings(object sender, EventArgs e)
    {
        try
        {
            var page = ServiceHost.Services?.GetService<SettingsPage>();
            var shell = Shell.Current;
            if (page != null && shell != null && shell.CurrentPage?.GetType() != page.GetType())
                await shell.Navigation.PushAsync(page);
        }
        catch
        {
            // fallback: show error
            await DisplayAlert(AppResources.NavigationError_Title, 
                AppResources.NavigationError_Settings, 
                AppResources.Ok);
        }
    }

    private async void OnOpenPlaybackSettings(object sender, EventArgs e)
    {
        try
        {
            var page = ServiceHost.Services?.GetService<PlaybackSettingsPage>();
            var shell = Shell.Current;
            if (page != null && shell != null && shell.CurrentPage?.GetType() != page.GetType())
                await shell.Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            await DisplayAlert(AppResources.NavigationError_Title, 
                ex.Message, 
                AppResources.Ok);
        }
    }
}