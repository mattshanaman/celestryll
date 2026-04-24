using Pemdas.ViewModels;

namespace Pemdas.Pages;

public partial class TestModePage : ContentPage
{
    public TestModePage(TestModeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is TestModeViewModel viewModel)
        {
            await viewModel.LoadPreviewCommand.ExecuteAsync(null);
        }
    }
}
