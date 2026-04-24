using Pemdas.ViewModels;

namespace Pemdas.Pages;

public partial class PastGamesPage : ContentPage
{
    private readonly PastGamesViewModel _viewModel;

    public PastGamesPage(PastGamesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
