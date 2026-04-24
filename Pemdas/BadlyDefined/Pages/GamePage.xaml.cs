using BadlyDefined.ViewModels;

namespace BadlyDefined.Pages;

public partial class GamePage : ContentPage
{
    private readonly GameViewModel _viewModel;

    public GamePage(GameViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            System.Diagnostics.Debug.WriteLine("🎮 GamePage.OnAppearing - Starting initialization");
            await _viewModel.InitializeAsync();
            System.Diagnostics.Debug.WriteLine("✅ GamePage initialization completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ GamePage initialization failed: {ex.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"❌ Message: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"❌ Stack: {ex.StackTrace}");

            // Show error to user
            await DisplayAlertAsync("Initialization Error", 
                $"Failed to load game: {ex.Message}\n\nPlease restart the app.", 
                "OK");
        }
    }
}
