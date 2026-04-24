# Advertising Implementation for Free Tier

## Overview

Comprehensive ad integration for Free tier users following these specifications:
- **45-minute playback limit** (15 min base + 30 min with rewarded ad extension option)
- **Banner ads** on non-playback screens (Library, Settings, Timer, Help)
- **Rewarded ads** to unlock longer sessions and premium sounds
- **Interstitial ads** only when opening app or ending sessions

---

## Architecture

### Services Created

#### 1. `IAdvertisingService` / `AdvertisingService`
Main service coordinating all ad operations:
- Banner ad display/hide
- Interstitial ad scheduling (with 5-minute cooldown)
- Rewarded ad presentation
- Platform-specific implementations via partial methods
- Ad initialization and preloading

#### 2. `IAdRewardManager` / `AdRewardManager`
Manages Free tier user rewards:
- **Session Extensions:** 45-minute grants from rewarded ads
- **Sound Unlocks:** Per-session premium sound access
- **Tracking:** Expiration times and unlocked content
- **Lifecycle:** Clears on session end

#### 3. `BannerAdView` Control
Reusable XAML control for banner ads:
- Automatic visibility based on tier
- Page-aware banner placement
- 50px height standard banner
- Self-hiding when not needed

---

## Implementation Details

### Free Tier Playback Limits

**Base Limit:** 15 minutes  
**With Ad Extension:** 45 additional minutes (60 total)

```csharp
// In FeatureGate.cs
public TimeSpan MaxSessionLength()
{
    if (Tier == SubscriptionTier.Free)
    {
        var baseTime = TimeSpan.FromMinutes(15);
        if (_adRewardManager?.HasSessionExtension == true)
        {
            return baseTime + _adRewardManager.RemainingExtensionTime;
        }
        return baseTime;
    }
    // ... other tiers
}
```

### Banner Ad Placement

**Show On:**
- Library Page
- Settings Page
- Timer Page
- Help Page
- Legal Page

**Hide On:**
- Playback Page (uninterrupted listening experience)

**Implementation:**
```xaml
<!-- Add to any page where banner should appear -->
<controls:BannerAdView PageType="Library" />
```

### Interstitial Ad Triggers

**App Open:**
- Shown when app launches (not first launch)
- 5-minute cooldown between interstitials
- Implemented in `App.OnStart()`

**Session End:**
- Shown when user manually stops playback
- Shown when timer expires
- Shown when session limit reached
- Implemented in `PlaybackViewModel.StopAllCommand`

**Cooldown Logic:**
```csharp
private readonly TimeSpan _interstitialCooldown = TimeSpan.FromMinutes(5);

public async Task ShowInterstitialAsync(string trigger)
{
    if (_lastInterstitialTime.HasValue)
    {
        var timeSince = DateTime.UtcNow - _lastInterstitialTime.Value;
        if (timeSince < _interstitialCooldown)
            return; // Skip - too soon
    }
    
    await ShowPlatformInterstitialAsync(trigger);
    _lastInterstitialTime = DateTime.UtcNow;
}
```

### Rewarded Ads

**Reward Types:**

1. **Extend Session** (`"ExtendSession"`)
   - Grants: 45 additional minutes
   - Stacks with current extension
   - Offered when approaching time limit

2. **Unlock Sound** (`"UnlockSound"`)
   - Grants: Access to one premium sound bundle for current session
   - Cleared when app closes or session ends
   - Offered when tapping locked bundles

**Implementation:**
```csharp
// Show rewarded ad
var completed = await _adService.ShowRewardedAdAsync("ExtendSession");
if (completed)
{
    // User gets reward
    _rewardManager.GrantSessionExtension(TimeSpan.FromMinutes(45));
}
```

---

## Integration with Existing Code

### FeatureGate Updates

Added ad-aware features:
```csharp
public bool ShouldShowAds => Tier == SubscriptionTier.Free;

public bool IsSoundBundleAccessible(string bundleId)
{
    if (Tier != SubscriptionTier.Free)
        return true;
    
    return _adRewardManager?.IsSoundUnlocked(bundleId) == true;
}
```

### MauiProgram.cs Registration

```csharp
builder.Services.AddSingleton<IAdRewardManager, AdRewardManager>();
builder.Services.AddSingleton<IAdvertisingService, AdvertisingService>();

builder.Services.AddSingleton<FeatureGate>(sp => 
    new FeatureGate(
        sp.GetRequiredService<ISubscriptionService>(),
        sp.GetRequiredService<IAdRewardManager>()));
```

### App Lifecycle

```csharp
protected override async void OnStart()
{
    if (_adService?.ShouldShowAds == true)
    {
        await _adService.InitializeAsync();
        
        if (!_isFirstLaunch)
            await _adService.ShowInterstitialAsync("AppOpen");
        
        _ = Task.Run(() => _adService.PreloadAdsAsync());
    }
}
```

---

## Platform-Specific Implementation Required

### Partial Methods to Implement

The `AdvertisingService` uses partial methods that need platform-specific implementations:

**For Android (using Google AdMob):**
```csharp
// Platforms/Android/Services/AdvertisingService.Android.cs
public partial class AdvertisingService
{
    partial void InitializePlatformAdsAsync()
    {
        // Initialize Google Mobile Ads SDK
        MobileAds.Initialize(Android.App.Application.Context);
    }
    
    partial void ShowPlatformBanner(string pageType)
    {
        // Show AdMob banner
    }
    
    partial void HidePlatformBanner()
    {
        // Hide AdMob banner
    }
    
    partial void ShowPlatformInterstitialAsync(string trigger)
    {
        // Load and show interstitial
    }
    
    partial void ShowPlatformRewardedAdAsync(string rewardType)
    {
        // Load and show rewarded video
    }
    
    partial void IsPlatformRewardedAdAvailable(string rewardType)
    {
        // Check if rewarded ad is loaded
    }
    
    partial void PreloadPlatformAdsAsync()
    {
        // Preload banner, interstitial, and rewarded ads
    }
}
```

**For iOS (using Google AdMob):**
Similar implementation in `Platforms/iOS/Services/AdvertisingService.iOS.cs`

---

## Ad Unit IDs Setup

### Required Ad Units

**Android (AdMob):**
- Banner: `ca-app-pub-XXXXXXXX/NNNNNNNNNN`
- Interstitial: `ca-app-pub-XXXXXXXX/NNNNNNNNNN`
- Rewarded: `ca-app-pub-XXXXXXXX/NNNNNNNNNN`

**iOS (AdMob):**
- Banner: `ca-app-pub-XXXXXXXX/NNNNNNNNNN`
- Interstitial: `ca-app-pub-XXXXXXXX/NNNNNNNNNN`
- Rewarded: `ca-app-pub-XXXXXXXX/NNNNNNNNNN`

**Test IDs (for development):**
```csharp
#if DEBUG
    private const string BannerId = "ca-app-pub-3940256099942544/6300978111";
    private const string InterstitialId = "ca-app-pub-3940256099942544/1033173712";
    private const string RewardedId = "ca-app-pub-3940256099942544/5224354917";
#else
    private const string BannerId = "ca-app-pub-REAL-ID-HERE";
    // ... production IDs
#endif
```

---

## NuGet Packages Required

### Android
```xml
<PackageReference Include="Xamarin.Google.Android.Play.Services.Ads" Version="23.0.0" />
```

### iOS
```xml
<PackageReference Include="Xamarin.Google.iOS.MobileAds" Version="11.0.0" />
```

### Both Platforms
```xml
<PackageReference Include="Plugin.AdMob" Version="2.0.0" />
<!-- OR implement custom native bindings -->
```

---

## Configuration Files

### Android Manifest
```xml
<application>
    <meta-data
        android:name="com.google.android.gms.ads.APPLICATION_ID"
        android:value="ca-app-pub-XXXXXXXX~NNNNNNNNNN"/>
</application>

<uses-permission android:name="android.permission.INTERNET"/>
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE"/>
```

### iOS Info.plist
```xml
<key>GADApplicationIdentifier</key>
<string>ca-app-pub-XXXXXXXX~NNNNNNNNNN</string>

<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```

---

## UI Integration Examples

### Library Page with Banner
```xaml
<ContentPage>
    <Grid RowDefinitions="*,Auto">
        <!-- Main content -->
        <CollectionView Grid.Row="0" ... />
        
        <!-- Banner ad at bottom -->
        <controls:BannerAdView Grid.Row="1" 
                               PageType="Library"
                               IsVisible="{Binding ShouldShowAds}" />
    </Grid>
</ContentPage>
```

### Session Extension Prompt
```xaml
<Border IsVisible="{Binding IsNearTimeLimit}">
    <VerticalStackLayout>
        <Label Text="? 2 Minutes Remaining" />
        <Label Text="Watch an ad to get 45 more minutes" />
        <Button Text="Watch Ad" 
                Command="{Binding WatchAdForExtensionCommand}" />
    </VerticalStackLayout>
</Border>
```

### Premium Sound Lock
```xaml
<Border IsVisible="{Binding IsLocked}">
    <VerticalStackLayout>
        <Label Text="?? Premium Sound" />
        <Button Text="Watch Ad to Unlock" 
                Command="{Binding WatchAdToUnlockCommand}" />
        <Label Text="Or upgrade to Premium tier" />
    </VerticalStackLayout>
</Border>
```

---

## User Experience Flow

### First App Launch
1. User opens app (Free tier)
2. No interstitial shown (first launch grace)
3. Banners shown on Library, Settings, Timer
4. Playback page has no ads

### During Playback
1. User starts mix (15-minute timer starts)
2. At 12 minutes: "3 minutes left" notification
3. At 13 minutes: "Watch ad for 45 more minutes" button appears
4. User watches rewarded ad
5. Timer extended to 58 minutes (13 elapsed + 45 granted)
6. User enjoys extended session

### Session End
1. User stops playback or timer expires
2. Interstitial ad shown (if cooldown expired)
3. User returns to Library with banner

### Subsequent App Opens
1. User opens app
2. Interstitial shown (if >5 min since last interstitial)
3. App loads with banners on non-playback pages

---

## Testing Checklist

### Ad Display
- [ ] Banners appear on Library page (Free tier)
- [ ] Banners appear on Settings page (Free tier)
- [ ] Banners appear on Timer page (Free tier)
- [ ] Banners do NOT appear on Playback page
- [ ] Banners do NOT appear on paid tiers
- [ ] Banner height is 50px
- [ ] Banners hide when navigating away

### Interstitial Timing
- [ ] No interstitial on first app launch
- [ ] Interstitial on subsequent app opens
- [ ] Interstitial on session end
- [ ] 5-minute cooldown enforced
- [ ] No interstitials during playback

### Rewarded Ads
- [ ] "Extend Session" button appears near time limit
- [ ] Watching ad grants 45 minutes
- [ ] Extension stacks with current time
- [ ] "Unlock Sound" appears on premium bundles
- [ ] Unlocked sounds work until session end
- [ ] Unlocks cleared when app closes

### Edge Cases
- [ ] Ads don't show on paid tiers
- [ ] Graceful handling if ad fails to load
- [ ] Network offline - no crashes
- [ ] Rapid navigation doesn't show multiple interstitials
- [ ] Timer doesn't break if ad takes time to show

---

## Analytics & Monitoring

### Recommended Tracking

**Ad Events:**
- Ad requested
- Ad loaded successfully
- Ad failed to load
- Ad clicked
- Ad closed
- Rewarded ad completion
- Reward granted

**User Behavior:**
- Free users who watch ads vs upgrade
- Session extension usage rate
- Sound unlock usage rate
- Ad fatigue indicators

**Revenue Metrics:**
- eCPM (effective cost per thousand impressions)
- Fill rate
- CTR (click-through rate)
- Revenue per free user

---

## Resource Strings to Add

Add these to `AppResources.resx`:

```xml
<!-- Ads -->
<data name="Ad_WatchForExtension" xml:space="preserve">
    <value>Watch an ad to get 45 more minutes</value>
</data>
<data name="Ad_WatchToUnlock" xml:space="preserve">
    <value>Watch an ad to unlock this sound for today</value>
</data>
<data name="Ad_TimeAlmostUp" xml:space="preserve">
    <value>Time Almost Up</value>
</data>
<data name="Ad_ExtendSession" xml:space="preserve">
    <value>Extend Session</value>
</data>
<data name="Ad_UnlockSound" xml:space="preserve">
    <value>Unlock Sound</value>
</data>
<data name="Ad_OrUpgrade" xml:space="preserve">
    <value>Or upgrade to Premium for unlimited sessions</value>
</data>
<data name="Ad_SessionExtended" xml:space="preserve">
    <value>Session Extended!</value>
</data>
<data name="Ad_SessionExtendedMessage" xml:space="preserve">
    <value>You've earned 45 more minutes. Enjoy!</value>
</data>
<data name="Ad_SoundUnlocked" xml:space="preserve">
    <value>Sound Unlocked!</value>
</data>
<data name="Ad_SoundUnlockedMessage" xml:space="preserve">
    <value>You can use this sound until you close the app.</value>
</data>
<data name="Ad_LoadingAd" xml:space="preserve">
    <value>Loading ad...</value>
</data>
<data name="Ad_AdNotAvailable" xml:space="preserve">
    <value>Ad not available right now. Please try again later.</value>
</data>
```

---

## Files Created

| File | Purpose |
|------|---------|
| `Services/IAdvertisingService.cs` | Interface for ad operations |
| `Services/AdvertisingService.cs` | Cross-platform ad service |
| `Services/AdRewardManager.cs` | Tracks ad-based rewards |
| `Controls/BannerAdView.cs` | Reusable banner control (code-behind) |
| `Controls/BannerAdView.xaml` | Reusable banner control (XAML) |

## Files Modified

| File | Changes |
|------|---------|
| `Services/FeatureGate.cs` | Added ad integration, session extension logic |
| `MauiProgram.cs` | Registered ad services |
| `App.xaml.cs` | Initialize ads, show app open interstitial |

---

## Next Steps

### 1. Add AdMob NuGet Packages
```bash
dotnet add package Xamarin.Google.Android.Play.Services.Ads
dotnet add package Xamarin.Google.iOS.MobileAds
```

### 2. Implement Platform-Specific Ad Code
- Create `Platforms/Android/Services/AdvertisingService.Android.cs`
- Create `Platforms/iOS/Services/AdvertisingService.iOS.cs`
- Implement partial methods for each platform

### 3. Configure Ad Unit IDs
- Create AdMob account
- Register app and get Application ID
- Create ad units (Banner, Interstitial, Rewarded)
- Update configuration files

### 4. Add Banners to Pages
- Modify Library, Settings, Timer, Help pages
- Add `<controls:BannerAdView PageType="[PageName]" />` to XAML

### 5. Integrate Rewarded Ad Triggers
- Add "Watch Ad" buttons to PlaybackViewModel
- Show prompts at 2-3 minutes remaining
- Add unlock buttons to locked sound bundles

### 6. Add Session End Interstitials
- Modify `StopAllCommand` in PlaybackViewModel
- Show interstitial before clearing playback
- Respect cooldown period

### 7. Test Thoroughly
- Use AdMob test IDs during development
- Test all ad types and placements
- Verify cooldowns and rewards work
- Test on both Android and iOS

### 8. Submit for Review
- Ensure compliance with AdMob policies
- Provide privacy policy (ad data usage)
- Follow app store guidelines for ads

---

## Privacy & Compliance

### Required Disclosures

**Privacy Policy Must Include:**
- We use Google AdMob for advertising
- AdMob may collect device identifiers (Advertising ID)
- AdMob may use cookies and similar technologies
- Users can opt out via device settings
- Link to Google's privacy policy

**App Store Requirements:**
- Declare ad networks used (AdMob)
- Specify ad types (Banner, Interstitial, Rewarded)
- Note: Age rating may be affected

### GDPR/CCPA Compliance
- Implement consent flow for EU users
- Use Google UMP (User Messaging Platform)
- Allow users to opt out of personalized ads
- Provide data deletion requests

---

## Estimated Revenue

**Assumptions:**
- 1,000 DAU (Daily Active Users) on Free tier
- 50% watch rewarded ads
- 30% see interstitials
- Average eCPM: $2-5

**Monthly Revenue:**
- Rewarded: 500 users × 30 days × 1 ad × $0.005 = $75
- Interstitial: 300 users × 30 days × 0.5 ads × $0.002 = $9
- Banner: 1,000 users × 30 days × 0.001 = $30

**Total: ~$114/month** from 1,000 Free tier users

Scales linearly with user base. Premium upgrade revenue will far exceed ad revenue.

---

**Implementation Date:** January 2025  
**Status:** ? Core Services Created  
**Platform Implementation:** ? Pending  
**Testing:** ? Pending  
**Production:** ? Pending  

## Summary

Comprehensive ad system created for Free tier with:
- ? Banner ads on non-playback screens
- ? Interstitial ads with smart timing
- ? Rewarded ads for session extension and sound unlocks
- ? 45-minute session management
- ? Clean architecture with platform abstraction
- ? Ready for platform-specific implementation

All core services and infrastructure are in place. Next step is implementing the platform-specific AdMob integration.
