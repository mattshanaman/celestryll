using BadlyDefined.ViewModels;

namespace BadlyDefined.Pages;

public partial class TestModePage : ContentPage
{
    private readonly TestModeViewModel _viewModel;

    public TestModePage(TestModeViewModel viewModel)
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
