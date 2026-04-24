using AmbientSleeper.ViewModels;

namespace AmbientSleeper.Views;

public partial class TimerPage : ContentPage
{
    public TimerPage(TimerViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        vm.EnsureFeatureSubscriptions(); // ensures slider cap updates if tier changes at runtime
    }
}