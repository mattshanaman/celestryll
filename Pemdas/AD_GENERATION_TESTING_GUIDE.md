# ?? Ad Generation & Testing Guide

## **When Do Ads Trigger?**

### **1. Interstitial Ads (Full-Screen)** ??

**Trigger**: After solving a puzzle correctly (non-subscribers only)

**Location**: `ViewModels/GameViewModel.cs`, line ~434-443

```csharp
if (isCorrect)
{
    // ... success feedback ...
    
    try
    {
        // ? AD TRIGGERS HERE!
        if (!await _subscriptionService.CheckSubscriptionStatus())
        {
            _adService.ShowInterstitialAd();
        }
    }
    catch (Exception adEx)
    {
        System.Diagnostics.Debug.WriteLine($"Error showing ad: {adEx.Message}");
    }
}
```

**Flow**:
1. User solves puzzle correctly ?
2. App checks subscription status
3. If NOT subscribed ? Show interstitial ad
4. If subscribed ? Skip ad

**Frequency**: Every correct answer (production would typically limit this)

---

### **2. Rewarded Ads (For Hint Tokens)** ??

**Trigger**: User taps "Watch Ad for Hint" button

**Location**: `ViewModels/GameViewModel.cs`, `WatchAdForHint()` command

```csharp
[RelayCommand]
private void WatchAdForHint()
{
    try
    {
        _feedbackService.LightTap();
        
        // ? AD TRIGGERS HERE!
        _adService.ShowRewardedAd(async () =>
        {
            try
            {
                // Reward: Give hint token
                HintTokens++;
                FeedbackMessage = AppResources.HintTokenEarned;
                ShowFeedback = true;
                await _feedbackService.PlaySuccessFeedback();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in ad reward callback: {ex.Message}");
            }
        });
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error showing rewarded ad: {ex.Message}");
    }
}
```

**Flow**:
1. User needs hint tokens
2. User taps button to watch ad
3. Ad plays
4. After watching ? User gets 1 hint token

**Frequency**: On-demand (user controlled)

---

## **Current Ad Implementation Status**

### **Status**: ?? **PLACEHOLDER MODE**

**Location**: `Services/AdService.cs`

```csharp
public void ShowInterstitialAd()
{
    // Plugin.MauiMTAdmob will be integrated after package restore
    // CrossMauiMTAdmob.Current.LoadInterstitial(adUnitId);
    System.Diagnostics.Debug.WriteLine("Interstitial ad requested (placeholder)");
}

public void ShowRewardedAd(Action? onRewarded)
{
    // Plugin.MauiMTAdmob will be integrated after package restore
    // For now, simulate ad completion for testing
    System.Diagnostics.Debug.WriteLine("Rewarded ad requested (placeholder)");
    OnRewardedVideoCompleted(onRewarded);  // ? Simulates reward immediately
}
```

### **What This Means**:

**Interstitial Ads**:
- ? Do NOT actually display
- ? Logs to debug console: `"Interstitial ad requested (placeholder)"`
- ? Code flow continues normally

**Rewarded Ads**:
- ? Do NOT actually display
- ? Immediately grants reward (simulates watching ad)
- ? User gets hint token without waiting
- ? Logs to debug console: `"Rewarded ad requested (placeholder)"`

---

## **How to Test Ad Logic NOW**

### **Test 1: Interstitial Ad Trigger**

**Steps**:
1. Launch app
2. Solve today's puzzle correctly
3. Watch Debug Console

**Expected Console Output**:
```
[Pemdas] Interstitial ad requested (placeholder)
```

**What You're Testing**:
- ? Ad trigger location is correct
- ? Subscription check works
- ? Code doesn't crash
- ? Timing is appropriate (after success)

---

### **Test 2: Rewarded Ad Flow**

**Steps**:
1. Launch app
2. Use all your hint tokens (solve puzzles)
3. Tap "?? Hint" ? See "No hint tokens" message
4. Look for "Watch Ad for Hint" option (if implemented in UI)
5. Or call `WatchAdForHint()` command
6. Watch Debug Console

**Expected Console Output**:
```
[Pemdas] Rewarded ad requested (placeholder)
[Pemdas] Rewarded ad completed successfully
```

**Expected Result**:
- ? Hint tokens increase by 1
- ? Success message appears
- ? Can now use hint

---

### **Test 3: Subscription Ad Bypass**

**Steps**:
1. Launch app
2. Set subscription status to true:
   ```csharp
   // In SubscriptionService or test mode
   _isSubscribed = true;
   ```
3. Solve puzzle correctly
4. Watch Debug Console

**Expected Console Output**:
```
(No ad log should appear)
```

**What You're Testing**:
- ? Subscribers don't see ads
- ? Subscription check works
- ? Ad is properly skipped

---

## **How to Enable REAL Ads**

### **Step 1: Add AdMob NuGet Package**

**For Android**:
```bash
Install-Package Plugin.MauiMTAdmob
```

**For iOS**:
```bash
Install-Package Plugin.MauiMTAdmob
```

---

### **Step 2: Initialize AdMob**

**File**: `MauiProgram.cs`

Add before `return builder.Build();`:
```csharp
#if ANDROID || IOS
builder.Services.AddSingleton<IMauiInitializeService, AdMobInitializer>();
#endif
```

**File**: `Services/AdService.cs`

Update `Initialize()`:
```csharp
public void Initialize()
{
    lock (_initLock)
    {
        if (_isInitialized)
            return;

        try
        {
            // ? Uncomment this line:
            CrossMauiMTAdmob.Current.Init();
            
            _isInitialized = true;
            System.Diagnostics.Debug.WriteLine("AdService initialized successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ad initialization failed: {ex.Message}");
            _isInitialized = false;
        }
    }
}
```

---

### **Step 3: Update AndroidManifest.xml**

**File**: `Platforms/Android/AndroidManifest.xml`

Add inside `<application>` tag:
```xml
<meta-data
    android:name="com.google.android.gms.ads.APPLICATION_ID"
    android:value="ca-app-pub-3940256099942544~3347511713"/>
```

**Note**: This is a TEST App ID. Replace with your real AdMob App ID for production.

---

### **Step 4: Update Info.plist (iOS)**

**File**: `Platforms/iOS/Info.plist`

Add:
```xml
<key>GADApplicationIdentifier</key>
<string>ca-app-pub-3940256099942544~1458002511</string>
<key>SKAdNetworkItems</key>
<array>
    <dict>
        <key>SKAdNetworkIdentifier</key>
        <string>cstr6suwn9.skadnetwork</string>
    </dict>
</array>
```

**Note**: This is a TEST App ID. Replace with your real AdMob App ID for production.

---

### **Step 5: Implement Ad Loading**

**File**: `Services/AdService.cs`

Update `ShowInterstitialAd()`:
```csharp
public void ShowInterstitialAd()
{
    if (!_isInitialized)
    {
        System.Diagnostics.Debug.WriteLine("Cannot show interstitial ad: AdService not initialized");
        return;
    }

    try
    {
        var adUnitId = GetInterstitialAdUnitId();
        if (string.IsNullOrEmpty(adUnitId))
        {
            System.Diagnostics.Debug.WriteLine("No ad unit ID configured for this platform");
            return;
        }

        // ? Uncomment these lines:
        CrossMauiMTAdmob.Current.LoadInterstitial(adUnitId);
        CrossMauiMTAdmob.Current.ShowInterstitial();
        
        System.Diagnostics.Debug.WriteLine("Interstitial ad shown");
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Failed to show interstitial ad: {ex.Message}");
    }
}
```

Update `ShowRewardedAd()`:
```csharp
public void ShowRewardedAd(Action? onRewarded)
{
    if (!_isInitialized)
    {
        System.Diagnostics.Debug.WriteLine("Cannot show rewarded ad: AdService not initialized");
        return;
    }

    try
    {
        var adUnitId = GetRewardedAdUnitId();
        if (string.IsNullOrEmpty(adUnitId))
        {
            System.Diagnostics.Debug.WriteLine("No rewarded ad unit ID configured for this platform");
            return;
        }

        // ? Uncomment these lines:
        CrossMauiMTAdmob.Current.OnRewardedVideoAdCompleted += (sender, e) => 
        {
            OnRewardedVideoCompleted(onRewarded);
        };
        
        CrossMauiMTAdmob.Current.LoadRewardedVideo(adUnitId);
        CrossMauiMTAdmob.Current.ShowRewardedVideo();
        
        System.Diagnostics.Debug.WriteLine("Rewarded ad shown");
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Failed to show rewarded ad: {ex.Message}");
    }
}
```

---

## **Testing with TEST Ad Unit IDs**

### **Current Ad Unit IDs** (Google Test IDs)

**Android**:
```csharp
Interstitial: ca-app-pub-3940256099942544/1033173712
Rewarded:     ca-app-pub-3940256099942544/5224354917
```

**iOS**:
```csharp
Interstitial: ca-app-pub-3940256099942544/4411468910
Rewarded:     ca-app-pub-3940256099942544/1712485313
```

**These are Google's official test IDs**:
- ? Safe to use for development
- ? Will show test ads
- ? Won't violate AdMob policies
- ? Can click without risk

**?? IMPORTANT**: Replace these with YOUR real Ad Unit IDs before production release!

---

## **How to Get REAL Ad Unit IDs**

### **Step 1: Create AdMob Account**
1. Go to https://admob.google.com
2. Sign in with Google account
3. Create new app or add existing

### **Step 2: Create Ad Units**
1. In AdMob dashboard, go to "Apps"
2. Select your app
3. Click "Ad units"
4. Click "Add ad unit"
5. Choose "Interstitial" or "Rewarded"
6. Configure settings
7. Copy Ad Unit ID (format: `ca-app-pub-XXXXXXXXXXXXXXXX/YYYYYYYYYY`)

### **Step 3: Replace Test IDs**

**File**: `Services/AdService.cs`

```csharp
private string GetInterstitialAdUnitId()
{
#if ANDROID
    return "ca-app-pub-YOUR-REAL-ID-HERE/XXXXXXXXXX";  // ? Replace
#elif IOS
    return "ca-app-pub-YOUR-REAL-ID-HERE/XXXXXXXXXX";  // ? Replace
#else
    return string.Empty;
#endif
}

private string GetRewardedAdUnitId()
{
#if ANDROID
    return "ca-app-pub-YOUR-REAL-ID-HERE/XXXXXXXXXX";  // ? Replace
#elif IOS
    return "ca-app-pub-YOUR-REAL-ID-HERE/XXXXXXXXXX";  // ? Replace
#else
    return string.Empty;
#endif
}
```

---

## **Production Ad Best Practices**

### **1. Limit Ad Frequency**

**Don't show ads after EVERY puzzle**:
```csharp
private int _puzzlesSinceLastAd = 0;
private const int AD_FREQUENCY = 3;  // Show ad every 3 puzzles

if (isCorrect)
{
    _puzzlesSinceLastAd++;
    
    try
    {
        if (!await _subscriptionService.CheckSubscriptionStatus() 
            && _puzzlesSinceLastAd >= AD_FREQUENCY)
        {
            _adService.ShowInterstitialAd();
            _puzzlesSinceLastAd = 0;  // Reset counter
        }
    }
    catch (Exception adEx)
    {
        System.Diagnostics.Debug.WriteLine($"Error showing ad: {adEx.Message}");
    }
}
```

---

### **2. Preload Ads**

Load ads in advance so they're ready:
```csharp
public async Task InitializeAsync()
{
    // ... existing initialization ...
    
    // Preload interstitial ad
    if (!await _subscriptionService.CheckSubscriptionStatus())
    {
        _adService.PreloadInterstitialAd();
    }
}
```

---

### **3. Handle Ad Load Failures**

```csharp
CrossMauiMTAdmob.Current.OnInterstitialLoaded += (sender, e) =>
{
    _isAdReady = true;
    System.Diagnostics.Debug.WriteLine("Ad loaded successfully");
};

CrossMauiMTAdmob.Current.OnInterstitialFailedToLoad += (sender, e) =>
{
    _isAdReady = false;
    System.Diagnostics.Debug.WriteLine($"Ad failed to load: {e.ErrorCode}");
    // Continue without ad - don't block user
};
```

---

### **4. Respect User Experience**

**Don't show ads**:
- ? During active gameplay
- ? When timer is running
- ? Before user sees result
- ? Too frequently (annoying!)

**Do show ads**:
- ? After puzzle completion
- ? At natural breaks
- ? With clear skip options (rewarded ads)
- ? Not more than once per 3-5 puzzles

---

## **Debug Console Logs**

### **What to Watch For**

**Successful Flow** (with real ads):
```
[Pemdas] AdService initialized successfully
[Pemdas] Ad loaded successfully
[Pemdas] Interstitial ad shown
[Pemdas] Rewarded ad shown
[Pemdas] Rewarded ad completed successfully
```

**Error Scenarios**:
```
[Pemdas] Cannot show interstitial ad: AdService not initialized
[Pemdas] No ad unit ID configured for this platform
[Pemdas] Failed to show interstitial ad: [error message]
[Pemdas] Ad failed to load: No Fill
```

---

## **Testing Checklist**

### **Phase 1: Logic Testing** (Current - Placeholder Mode)
- [x] Interstitial ad triggers after correct answer
- [x] Rewarded ad triggers when requested
- [x] Subscribers skip interstitial ads
- [x] Ad errors don't crash app
- [x] Reward callback executes
- [x] Hint tokens increment correctly

### **Phase 2: Integration Testing** (After adding AdMob)
- [ ] AdMob SDK initializes
- [ ] Test ads load and display
- [ ] Interstitial ads close properly
- [ ] Rewarded ads grant rewards
- [ ] Ad frequency is reasonable
- [ ] Subscription bypass works

### **Phase 3: Production Testing**
- [ ] Real Ad Unit IDs configured
- [ ] Ads display in production
- [ ] Revenue tracking works
- [ ] User experience is smooth
- [ ] No policy violations
- [ ] Analytics integration

---

## **Quick Reference**

### **To Test Ad Triggers NOW**:
1. ? Run app in debug mode
2. ? Solve puzzle correctly
3. ? Watch Debug Console for: `"Interstitial ad requested (placeholder)"`
4. ? Verify subscription check works

### **To Enable Real Ads**:
1. ? Add `Plugin.MauiMTAdmob` NuGet package
2. ? Uncomment ad initialization code
3. ? Update AndroidManifest.xml and Info.plist
4. ? Get real Ad Unit IDs from AdMob
5. ? Replace test IDs with real IDs
6. ? Test with real ads
7. ? Deploy to production

---

## **Summary**

### **Current State** ??
- Ad trigger points: ? Implemented
- Ad logic flow: ? Correct
- Subscription check: ? Working
- Actual ads: ? Placeholder mode

### **When Ads Trigger** ??
1. **Interstitial**: After solving puzzle (non-subscribers)
2. **Rewarded**: On-demand when user wants hint token

### **Testing** ??
- ? Can test trigger logic NOW via Debug Console
- ? Need AdMob SDK to test actual ads
- ? All error handling in place
- ? Safe to deploy with placeholders

### **Next Steps** ??
1. Test current placeholder logic
2. Verify ad triggers at right times
3. Add AdMob package when ready
4. Configure real Ad Unit IDs
5. Test with real ads
6. Fine-tune frequency
7. Deploy to production

---

**Date**: December 19, 2024  
**Status**: Ad triggers implemented, placeholders active  
**Ready for**: Logic testing now, SDK integration later  
**Test IDs**: Google official test IDs configured  
**Production**: Replace test IDs before release
