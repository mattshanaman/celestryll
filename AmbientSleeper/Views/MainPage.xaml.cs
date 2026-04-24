using AmbientSleeper.ViewModels;

namespace AmbientSleeper.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MainViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;

		//Page = new ContentPage()
		//{
		//	Content = new VerticalStackLayout
		//	{
		//		Children = {
		//			new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
		//			}
		//		}
		//	}
		//};
	}
}