using AmbientSleeper.ViewModels;

namespace AmbientSleeper.Views;

public partial class EqPage : ContentPage
{
    public EqPage(EqViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}