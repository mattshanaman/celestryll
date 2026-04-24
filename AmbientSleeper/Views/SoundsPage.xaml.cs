namespace AmbientSleeper.Views;

public partial class SoundsPage : ContentPage
{
	public SoundsPage(ViewModels.SoundsViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}