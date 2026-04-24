using AmbientSleeper.Services;
using Microsoft.Maui.Controls;

namespace AmbientSleeper.Controls;

/// <summary>
/// Cross-platform banner ad control
/// Simple implementation without XAML
/// </summary>
public class BannerAdView : ContentView
{
    private readonly IAdvertisingService? _adService;
    private readonly Grid _container;
    
    public static readonly BindableProperty PageTypeProperty =
        BindableProperty.Create(
            nameof(PageType),
            typeof(string),
            typeof(BannerAdView),
            default(string),
            propertyChanged: OnPageTypeChanged);
    
    public string PageType
    {
        get => (string)GetValue(PageTypeProperty);
        set => SetValue(PageTypeProperty, value);
    }
    
    public BannerAdView()
    {
        // Create container
        _container = new Grid
        {
            BackgroundColor = Colors.LightGray,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.End
        };
        
        Content = _container;
        
        // Get ad service from DI
        _adService = ServiceHost.Services?.GetService<IAdvertisingService>();
        
        // Set initial visibility
        UpdateVisibility();
    }
    
    private static void OnPageTypeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BannerAdView adView)
        {
            adView.UpdateBanner();
        }
    }
    
    private void UpdateVisibility()
    {
        if (_adService is null)
        {
            IsVisible = false;
            return;
        }
        
        IsVisible = _adService.ShouldShowAds && _adService.AreAdsReady;
        HeightRequest = IsVisible ? 50 : 0; // Standard banner height
    }
    
    private void UpdateBanner()
    {
        UpdateVisibility();
        
        if (!IsVisible || _adService is null || string.IsNullOrWhiteSpace(PageType))
            return;
        
        _adService.ShowBanner(PageType);
    }
    
    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        UpdateBanner();
    }
    
    protected override void OnParentSet()
    {
        base.OnParentSet();
        
        if (Parent != null)
        {
            UpdateBanner();
        }
        else if (_adService is not null)
        {
            _adService.HideBanner();
        }
    }
}
