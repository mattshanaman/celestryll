using AmbientSleeper.Models;
using AmbientSleeper.Services;
using AmbientSleeper.ViewModels;
using System.Linq;
using AmbientSleeper.Resources.Strings;

namespace AmbientSleeper.Views;

public partial class LibraryPage : ContentPage
{
    public LibraryPage(LibraryViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnAddToPlaylistClicked(object? sender, EventArgs e)
    {
        var vm = BindingContext as LibraryViewModel;
        if (vm == null)
            return;

        // strict gate — show prompt on Free
        if (!vm.IsPlaylistEnabled)
        {
            await DisplayAlert(AppResources.PlaylistLocked_Title,
                AppResources.PlaylistLocked_Message,
                AppResources.Ok);
            return;
        }

        if (sender is Button btn && btn.CommandParameter is AudioItem sound)
        {
            // If no playlists exist, create a Default playlist
            if (vm.Playlists.Count == 0)
            {
                var defaultPlaylist = new SavedPlaylist
                {
                    Name = "Default",
                    Items = new List<AudioItem>(),
                    SavedAt = DateTime.UtcNow
                };
                UserPreferences.SavePlaylist(defaultPlaylist);
                vm.RefreshPlaylists();
                await DisplayAlert(AppResources.DefaultPlaylistCreated_Title, AppResources.DefaultPlaylistCreated_Message, AppResources.Ok);
            }

            // Show picker dialog
            var playlistNames = vm.Playlists.Select(p => p.Name).ToArray();
            string? chosen = await DisplayActionSheet(AppResources.SelectPlaylist_Title, AppResources.Cancel, null, playlistNames);

            if (!string.IsNullOrEmpty(chosen) && chosen != AppResources.Cancel)
            {
                vm.AddSoundToPlaylistCommand.Execute((sound, chosen));
                ShowConfirmationPopup();
            }
        }
    }

    // New handler for badge-style Grid with TapGestureRecognizer
    private async void OnAddToPlaylistTapped(object? sender, TappedEventArgs e)
    {
        var vm = BindingContext as LibraryViewModel;
        if (vm == null)
            return;

        if (!vm.IsPlaylistEnabled)
        {
            await DisplayAlert(AppResources.PlaylistLocked_Title,
                AppResources.PlaylistLocked_Message,
                AppResources.Ok);
            return;
        }

        var sound = (e.Parameter as AudioItem) ?? (sender as BindableObject)?.BindingContext as AudioItem;
        if (sound == null)
            return;

        if(vm.Playlists.Count == 0)
        {
            var defaultPlaylist = new SavedPlaylist
            {
                Name = "Default",
                Items = new List<AudioItem>(),
                SavedAt = DateTime.UtcNow
            };
            UserPreferences.SavePlaylist(defaultPlaylist);
            vm.RefreshPlaylists();
            await DisplayAlert(AppResources.DefaultPlaylistCreated_Title, AppResources.DefaultPlaylistCreated_Message, AppResources.Ok);
        }

        var playlistNames = vm.Playlists.Select(p => p.Name).ToArray();
        string? chosen = await DisplayActionSheet(AppResources.SelectPlaylist_Title, AppResources.Cancel, null, playlistNames);

        if (!string.IsNullOrEmpty(chosen) && chosen != AppResources.Cancel)
        {
            vm.AddSoundToPlaylistCommand.Execute((sound, chosen));
            ShowConfirmationPopup();
        }
    }

    private void OnSaveMixPlaylistFromLibrary(object? sender, EventArgs e)
    {
        if (BindingContext is not LibraryViewModel vm)
            return;

        var entry = this.FindByName<Entry>("NewMixPlaylistNameEntry");
        var name = entry?.Text ?? string.Empty;
        vm.SaveMixPlaylistFromLibraryCommand.Execute(name);
        if (entry != null)
            entry.Text = string.Empty;
    }

    private async void ShowConfirmationPopup()
    {
        var popup = ConfirmationPopup;
        if (popup == null) return;
        popup.IsVisible = true;
        await Task.Delay(1500);
        popup.IsVisible = false;
    }
}