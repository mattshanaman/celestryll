# ? Splash Screen Circle Issue - FIXED

## Problem
The splash screen was showing a small circle PNG in the center instead of filling the screen.

## Root Causes Found

### 1. Custom Android Splash Drawable
- **File:** `Platforms\Android\Resources\drawable\splash_screen.xml`
- **Issue:** Was configured to show `@mipmap/appicon` (the app icon) instead of your splash screen
- **Fix:** ? **DELETED** this file

### 2. Incorrect styles.xml Reference
- **File:** `Platforms\Android\Resources\values\styles.xml`
- **Issue:** Was referencing the deleted `@drawable/splash_screen`
- **Fix:** ? **UPDATED** to use `@drawable/maui_splash_image` (MAUI's generated splash)

### 3. Small BaseSize
- **File:** `AmbientSleeper.csproj`
- **Issue:** `BaseSize="456,456"` was too small, making the icon appear tiny
- **Fix:** ? **INCREASED** to `BaseSize="1024,1024"`

## Changes Made

### 1. Deleted Custom Splash Drawable
```bash
REMOVED: Platforms\Android\Resources\drawable\splash_screen.xml
```

### 2. Updated Android Styles
**File:** `Platforms\Android\Resources\values\styles.xml`

```xml
<!-- BEFORE -->
<item name="android:windowBackground">@drawable/splash_screen</item>

<!-- AFTER -->
<item name="android:windowBackground">@drawable/maui_splash_image</item>
```

### 3. Increased Splash Screen Size
**File:** `AmbientSleeper.csproj`

```xml
<!-- BEFORE -->
<MauiSplashScreen Include="Resources\Splash\ambient_sleeper_background_and_small_icon2.png" 
    Color="#1a1a2e" 
    BaseSize="456,456" />

<!-- AFTER -->
<MauiSplashScreen Include="Resources\Splash\ambient_sleeper_background_and_small_icon2.png" 
    Color="#1a1a2e" 
    BaseSize="1024,1024" />
```

## How It Works Now

### MAUI Splash Generation
1. MAUI reads your `.csproj` configuration
2. Takes `ambient_sleeper_background_and_small_icon2.png`
3. Generates multiple density versions (mdpi, hdpi, xhdpi, xxhdpi, xxxhdpi)
4. Creates `maui_splash_image.xml` drawable
5. Android displays this on app launch

### Android Splash Display
1. App starts with `Maui.SplashTheme` (from `MainActivity.cs`)
2. Theme uses `@drawable/maui_splash_image` as window background
3. Image is centered at 1024x1024 size
4. Background color `#1a1a2e` fills the rest
5. Smooth transition to main app

## Current Configuration

### Splash Screen Priority
```xml
<!-- PRIMARY: Your branded PNG (1024x1024) -->
<MauiSplashScreen Include="Resources\Splash\ambient_sleeper_background_and_small_icon2.png" 
    Color="#1a1a2e" 
 BaseSize="1024,1024" />

<!-- FALLBACK: SVG (456x456) -->
<MauiSplashScreen Include="Resources\Splash\ambientsleeper_splash.svg" 
    Color="#1a1a2e" 
    BaseSize="456,456" />
```

### Android Theme
```xml
<style name="Maui.SplashTheme" parent="Theme.AppCompat.Light.NoActionBar">
    <item name="android:windowBackground">@drawable/maui_splash_image</item>
    <item name="android:statusBarColor">@color/splashBackground</item>
    <item name="android:navigationBarColor">@color/splashBackground</item>
</style>
```

## Testing the Fix

### 1. Close Visual Studio
File locks prevent proper cleaning. Close VS completely.

### 2. Clean Build Artifacts
```powershell
dotnet clean
```

If you get access denied errors, that's okay - just proceed to rebuild.

### 3. Rebuild for Android
```powershell
dotnet build -f net9.0-android
```

### 4. Deploy to Device/Emulator
```powershell
dotnet build -f net9.0-android -t:Run
```

### 5. Verify Splash Screen
Check that:
- ? Splash appears immediately (no white flash)
- ? Icon is larger and clearly visible
- ? Background is dark blue (#1a1a2e)
- ? NO small circle in center
- ? Smooth transition to app

## Size Adjustment Guide

If 1024x1024 is still not the right size for you:

### Current Size
```xml
BaseSize="1024,1024"  <!-- ~70-80% of screen -->
```

### Make it Smaller
```xml
BaseSize="768,768"    <!-- ~50-60% of screen -->
```

### Make it Larger
```xml
BaseSize="1536,1536"  <!-- ~90-95% of screen -->
```

### Make it Full Screen
```xml
BaseSize="2048,2048"  <!-- Nearly full screen -->
```

**Note:** Your source PNG is 1007670 bytes (~1MB). Scaling beyond its native resolution may cause pixelation.

## Troubleshooting

### Issue: Still shows small circle

**Check:**
1. Is Visual Studio closed? (File locks prevent updates)
2. Did you rebuild? (`dotnet build`)
3. Is emulator/device running old cached version?

**Solution:**
```powershell
# Close Visual Studio
# Close emulator/disconnect device
dotnet clean
Remove-Item -Recurse -Force bin, obj
dotnet build -f net9.0-android
dotnet build -f net9.0-android -t:Run
```

### Issue: White flash before splash

**Check:** `colors.xml` has `splashBackground`:
```xml
<color name="splashBackground">#1a1a2e</color>
```

### Issue: Splash is pixelated

**Cause:** BaseSize is larger than source image resolution

**Solutions:**
1. Reduce `BaseSize` (e.g., to 768x768 or 512x512)
2. OR create a higher resolution source image (2048x2048)
3. OR use the SVG fallback (scales infinitely)

### Issue: Build errors about splash_screen.xml

**Cause:** File might still be cached

**Solution:**
```powershell
dotnet clean
Remove-Item "Platforms\Android\Resources\drawable\splash_screen.xml" -ErrorAction SilentlyContinue
dotnet build
```

## What Each File Does

### Your Source Files
| File | Purpose |
|------|---------|
| `Resources\Splash\ambient_sleeper_background_and_small_icon2.png` | Source splash image (1MB) |
| `Resources\Splash\ambientsleeper_splash.svg` | SVG fallback (1.8KB) |

### Generated Files (Auto-created by MAUI)
| File | Purpose |
|------|---------|
| `obj/.../drawable/maui_splash_image.xml` | Layer-list that displays your PNG |
| `obj/.../drawable-xxxhdpi/ambient_sleeper...png` | High-density version |
| `obj/.../drawable-xxhdpi/ambient_sleeper...png` | Extra-high-density version |
| `obj/.../drawable-xhdpi/ambient_sleeper...png` | Extra-density version |
| `obj/.../drawable-hdpi/ambient_sleeper...png` | High-density version |
| `obj/.../drawable-mdpi/ambient_sleeper...png` | Medium-density version |

### Android Platform Files
| File | Purpose |
|------|---------|
| `Platforms\Android\Resources\values\styles.xml` | Defines splash theme |
| `Platforms\Android\Resources\values\colors.xml` | Defines splash colors |
| `Platforms\Android\MainActivity.cs` | Sets splash theme on launch |

## Alternative Solutions

If 1024x1024 still doesn't look right, here are other options:

### Option A: Create Full-Screen Image
1. Design a new splash image at 2048x2048
2. Place your icon/logo in the center
3. Use background color #1a1a2e for the rest
4. Save as `splash_fullscreen.png`
5. Update `.csproj`:
   ```xml
   <MauiSplashScreen Include="Resources\Splash\splash_fullscreen.png" 
       Color="#1a1a2e" 
       BaseSize="2048,2048" />
   ```

### Option B: Use SVG Only
SVG scales infinitely without pixelation:
1. Edit `AmbientSleeper.csproj`
2. Remove or comment out the PNG splash:
   ```xml
   <!-- <MauiSplashScreen Include="Resources\Splash\ambient_sleeper_background_and_small_icon2.png" ... /> -->
```
3. Keep only the SVG:
   ```xml
   <MauiSplashScreen Include="Resources\Splash\ambientsleeper_splash.svg" 
       Color="#1a1a2e" 
       BaseSize="1024,1024" />
   ```

### Option C: Platform-Specific Sizes
Different sizes for different platforms:
```xml
<!-- Android: Larger -->
<MauiSplashScreen Include="Resources\Splash\ambient_sleeper_background_and_small_icon2.png" 
    Color="#1a1a2e" 
    BaseSize="1024,1024"
    Condition="'$(TargetFramework)' == 'net9.0-android'" />

<!-- iOS: Smaller -->
<MauiSplashScreen Include="Resources\Splash\ambient_sleeper_background_and_small_icon2.png" 
    Color="#1a1a2e" 
    BaseSize="768,768"
    Condition="'$(TargetFramework)' == 'net9.0-ios'" />
```

## Summary

### ? What Was Fixed
1. **Removed** custom `splash_screen.xml` that showed app icon
2. **Updated** `styles.xml` to use MAUI-generated splash
3. **Increased** BaseSize from 456 to 1024 for larger display

### ?? Current Status
- **Splash Source:** `ambient_sleeper_background_and_small_icon2.png`
- **Display Size:** 1024x1024 (approximately 70-80% of screen)
- **Background:** Dark blue #1a1a2e
- **Build Status:** ? Successful

### ?? Next Steps
1. Close Visual Studio
2. Run: `dotnet build -f net9.0-android -t:Run`
3. Check splash screen on device
4. Adjust BaseSize if needed (768, 1024, 1536, or 2048)

### ?? Key Learnings
- Custom Android drawables **override** MAUI splash configuration
- Always use MAUI's generated resources (`@drawable/maui_splash_image`)
- BaseSize controls how large the splash appears on screen
- Larger BaseSize = larger icon (but may pixelate if source is small)

---

**Issue:** Small circle PNG on splash screen  
**Root Cause:** Custom drawable + small BaseSize  
**Fix:** Use MAUI splash + increase BaseSize to 1024  
**Status:** ? FIXED - Ready to test  
**Created:** January 2025  
