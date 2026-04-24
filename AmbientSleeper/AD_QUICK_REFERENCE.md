# Ad System Quick Reference

## For Developers

### Adding Banner to a Page
```xaml
<ContentPage>
    <Grid RowDefinitions="*,Auto">
        <!-- Main content -->
        <CollectionView Grid.Row="0" ... />
        
        <!-- Banner ad -->
        <controls:BannerAdView Grid.Row="1" PageType="YourPageName" />
    </Grid>
</ContentPage>
```

### Showing Rewarded Ad
```csharp
var adService = ServiceHost.Services?.GetService<IAdvertisingService>();
var completed = await adService.ShowRewardedAdAsync("ExtendSession");
if (completed)
{
    // Reward automatically granted by AdRewardManager
}
```

### Checking Session Extension
```csharp
var rewardManager = ServiceHost.Services?.GetService<IAdRewardManager>();
if (rewardManager.HasSessionExtension)
{
    var remaining = rewardManager.RemainingExtensionTime;
    // User has extended session
}
```

### Showing Interstitial
```csharp
var adService = ServiceHost.Services?.GetService<IAdvertisingService>();
await adService.ShowInterstitialAsync("SessionEnd");
```

---

## Ad Placement Rules

| Page | Banner | Interstitial | Rewarded |
|------|--------|--------------|----------|
| Library | ? Yes | ? No | On locked sounds |
| Playback | ? **NO** | ? No | Near time limit |
| Timer | ? Yes | ? No | ? No |
| Settings | ? Yes | ? No | ? No |
| Help | ? Yes | ? No | ? No |
| App Open | ? No | ? Yes (after 1st launch) | ? No |
| Session End | ? No | ? Yes | ? No |

---

## Tier Behavior

| Tier | Ads | Session Limit | Extension Available |
|------|-----|---------------|---------------------|
| Free | ? All types | 15 minutes | ? +45 min via ad |
| Standard | ? None | 2 hours | ? Not needed |
| Premium | ? None | 8 hours | ? Not needed |
| Pro+ | ? None | Unlimited | ? Not needed |

---

## Testing

### Use Test IDs
```csharp
#if DEBUG
    const string BANNER_ID = "ca-app-pub-3940256099942544/6300978111";
    const string INTERSTITIAL_ID = "ca-app-pub-3940256099942544/1033173712";
    const string REWARDED_ID = "ca-app-pub-3940256099942544/5224354917";
#endif
```

### Test Scenarios
1. **Free Tier with Ads:**
   - Set tier to Free
   - Verify banners appear on Library/Settings/Timer
   - Verify NO banners on Playback
   - Start playback and wait 13 minutes
   - Check for "Watch ad for more time" prompt

2. **Paid Tier (No Ads):**
   - Set tier to Standard/Premium/Pro+
   - Verify NO banners anywhere
   - Verify NO interstitials
   - Verify NO rewarded ad prompts

3. **Interstitial Cooldown:**
   - Show interstitial on app open
   - Close and reopen within 5 minutes
   - Verify NO interstitial (cooldown active)
   - Wait 5+ minutes
   - Reopen app
   - Verify interstitial shows

---

## Common Issues & Fixes

### Banner Not Showing
- Check if tier is Free: `_adService.ShouldShowAds`
- Check if ads initialized: `_adService.AreAdsReady`
- Check page type: Playback page should NOT show banner
- Check platform implementation exists

### Rewarded Ad Not Granting Reward
- Check `AdRewardManager.GrantSessionExtension()` called
- Check `FeatureGate.MaxSessionLength()` includes extension
- Verify timer UI updates after reward

### Interstitial Showing Too Often
- Check `GetInterstitialCooldown()` returns > 0
- Verify 5-minute cooldown enforced
- Check `_lastInterstitialTime` being set

### Session Extension Not Working
- Verify `AdRewardManager` injected into `FeatureGate`
- Check `HasSessionExtension` property
- Ensure `RemainingExtensionTime` > 0
- Verify timer respects extended time

---

## Platform Implementation Checklist

### Android
- [ ] Add NuGet: `Xamarin.Google.Android.Play.Services.Ads`
- [ ] Create `Platforms/Android/Services/AdvertisingService.Android.cs`
- [ ] Implement partial methods
- [ ] Update `AndroidManifest.xml` with App ID
- [ ] Add INTERNET permission
- [ ] Test with AdMob test IDs

### iOS
- [ ] Add NuGet: `Xamarin.Google.iOS.MobileAds`
- [ ] Create `Platforms/iOS/Services/AdvertisingService.iOS.cs`
- [ ] Implement partial methods
- [ ] Update `Info.plist` with App ID
- [ ] Configure App Transport Security
- [ ] Test with AdMob test IDs

---

## Key Files

| File | Purpose |
|------|---------|
| `Services/IAdvertisingService.cs` | Ad service interface |
| `Services/AdvertisingService.cs` | Cross-platform implementation |
| `Services/AdRewardManager.cs` | Reward tracking |
| `Controls/BannerAdView.cs` | Banner UI control |
| `Services/FeatureGate.cs` | Tier + ad integration |
| `App.xaml.cs` | App lifecycle & ads |
| `MauiProgram.cs` | Service registration |

---

## Ad Unit Configuration

Store in secure configuration or constants:

```csharp
public static class AdConfig
{
    #if DEBUG
    public const string ANDROID_BANNER = "ca-app-pub-3940256099942544/6300978111";
    public const string ANDROID_INTERSTITIAL = "ca-app-pub-3940256099942544/1033173712";
    public const string ANDROID_REWARDED = "ca-app-pub-3940256099942544/5224354917";
    
    public const string IOS_BANNER = "ca-app-pub-3940256099942544/2934735716";
    public const string IOS_INTERSTITIAL = "ca-app-pub-3940256099942544/4411468910";
    public const string IOS_REWARDED = "ca-app-pub-3940256099942544/1712485313";
    #else
    // Production IDs from AdMob account
    public const string ANDROID_BANNER = "ca-app-pub-YOUR-REAL-ID";
    // ... etc
    #endif
}
```

---

## Best Practices

### DO ?
- Initialize ads early (App.OnStart)
- Preload ads in background
- Respect interstitial cooldown
- Hide banners on Playback page
- Grant rewards immediately after completion
- Clear session rewards on app close
- Use test IDs during development
- Monitor fill rate and eCPM

### DON'T ?
- Show ads on paid tiers
- Interrupt active playback with ads
- Show interstitials too frequently
- Block UI while loading ads
- Forget to handle ad load failures
- Use production IDs in debug builds
- Neglect privacy policy updates
- Ignore GDPR/CCPA requirements

---

## Support

For issues or questions:
1. Check `ADVERTISING_IMPLEMENTATION_COMPLETE.md` for detailed docs
2. Check `AD_SYSTEM_IMPLEMENTATION_SUMMARY.md` for overview
3. Review platform-specific AdMob documentation
4. Test with AdMob test IDs first
5. Use debug logging to trace ad flow

---

**Quick Start:** See `ADVERTISING_IMPLEMENTATION_COMPLETE.md` for full guide
