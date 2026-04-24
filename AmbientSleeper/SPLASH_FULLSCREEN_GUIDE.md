# ??? Full-Screen Splash Screen Guide

## Current Issue

Your splash screen appears as a **small circle** in the center of the screen because:
1. The `BaseSize` is small (512x512)
2. MAUI creates adaptive icons with circular/rounded masks
3. Android defaults to centered, safe-area placement

---

## ? Solution: Multiple Options

### **Option 1: Design Full-Screen Image** (Recommended)

Create a new splash image designed for full-screen display.

#### **Image Specifications:**

**Dimensions:**
- **Android:** 1920x1920 px (covers all screen sizes)
- **iOS:** 2732x2732 px (iPad Pro max size)
- **Safe recommendation:** 2048x2048 px (works for both)

**Design Guidelines:**
- Place critical content in **center 1024x1024** area (safe zone)
- Background extends to full 2048x2048
- No important content in outer 512px border (may be cropped)

#### **File Format:**
- PNG with transparency
- Or PNG with solid background matching `Color="#1a1a2e"`

#### **Updated .csproj:**

```xml
<MauiSplashScreen Include="Resources\Splash\splash_fullscreen.png" 
 Color="#1a1a2e" 
       BaseSize="2048,2048" />
```

---

### **Option 2: Stretch Current Image** (Quick Fix)

Make your current 512x512 image fill more of the screen.

#### **Updated .csproj:**

```xml
<MauiSplashScreen Include="Resources\Splash\ambient_sleeper_background_and_small_icon2.png" 
    Color="#1a1a2e" 
       BaseSize="1024,1024"
                  Resize="False" />
```

**Changes:**
- `BaseSize="1024,1024"` - Larger size (will be upscaled)
- `Resize="False"` - Don't auto-resize (fills more space)

**Trade-off:** Image may appear pixelated on larger screens.

---

### **Option 3: Custom Android Drawable** (Advanced)

Override Android's splash screen generation with custom drawable.

#### **Step 1: Create custom splash drawable**

Create: `Platforms/Android/Resources/drawable/splash_background.xml`

```xml
<?xml version="1.0" encoding="utf-8"?>
<layer-list xmlns:android="http://schemas.android.com/apk/res/android">
    <!-- Background color -->
    <item android:drawable="@color/splashBackground" />
    
    <!-- Full-screen image -->
    <item>
        <bitmap
          android:src="@drawable/splash_image"
            android:gravity="fill"
  android:scaleType="centerCrop" />
    </item>
</layer-list>
```

#### **Step 2: Add image to drawable**

Copy your PNG to: `Platforms/Android/Resources/drawable/splash_image.png`

#### **Step 3: Update styles.xml**

```xml
<style name="Maui.SplashTheme" parent="Theme.AppCompat.Light.NoActionBar">
    <item name="android:windowBackground">@drawable/splash_background</item>
    <item name="android:windowNoTitle">true</item>
    <item name="android:windowFullscreen">true</item>
    <item name="android:windowContentOverlay">@null</item>
    <item name="colorPrimary">@color/splashBackground</item>
</style>
```

#### **Step 4: Remove from .csproj**

Comment out the MAUI splash screen for Android:

```xml
<!-- Only use for iOS -->
<MauiSplashScreen Include="Resources\Splash\ambient_sleeper_background_and_small_icon2.png" 
      Color="#1a1a2e" 
             BaseSize="512,512"
      Condition="'$(TargetFramework)' == 'net9.0-ios'" />
```

---

### **Option 4: Background Image + Icon** (Professional)

Separate background and foreground for adaptive display.

#### **Design Two Images:**

1. **Background:** 1920x1920 px solid color or gradient
2. **Icon/Logo:** 512x512 px (centered content)

#### **.csproj Configuration:**

```xml
<!-- Background layer -->
<MauiSplashScreen Include="Resources\Splash\splash_background.png" 
            Color="#1a1a2e" 
  BaseSize="1920,1920" />

<!-- Foreground icon/logo -->
<MauiImage Include="Resources\Splash\splash_icon.png" 
      BaseSize="512,512" 
        TintColor="#FFFFFF" />
```

---

## ?? Design Recommendations

### **For Your Ambient Sleeper App:**

Since it's a sleep/relaxation app, consider:

1. **Dark, calming background** (#1a1a2e - already perfect!)
2. **Centered branding** (your icon/logo in middle)
3. **Minimalist approach** (less is more for sleep apps)

### **Example Layout:**

```
???????????????????????????
??  ? Dark blue (#1a1a2e)
?     ?
?    ?????????     ?
?      ? ???  ?      ?  ? Your icon (512x512)
?      ?  Logo ?         ?     Centered
?      ?????????     ?
?           ?
?   Ambient Sleeper      ?  ? Optional text
?     ?
???????????????????????????
     Full screen 2048x2048
```

---

## ?? Platform Differences

### **Android:**
- Uses adaptive icons (circular/rounded masks)
- Splash shows in status bar area
- Recommended: 1920x1920 or 2048x2048

### **iOS:**
- Uses launch storyboard
- Splash fills entire screen
- Recommended: 2732x2732 (iPad Pro size)
- Scales down for iPhone

### **Common Size:**
- **2048x2048** works well for both
- Place critical content in center 1024x1024

---

## ??? Quick Implementation

### **For Quick Fix (Use Current Image):**

1. **Update .csproj:**

```xml
<MauiSplashScreen Include="Resources\Splash\ambient_sleeper_background_and_small_icon2.png" 
      Color="#1a1a2e" 
          BaseSize="1024,1024" />
```

2. **Rebuild:**
```powershell
dotnet clean
dotnet build
```

3. **Test on device**

**Result:** Larger splash image (may be slightly pixelated)

---

### **For Professional Full-Screen (New Image):**

1. **Design new 2048x2048 splash image**
   - Background: Full 2048x2048 with color #1a1a2e
   - Icon/Logo: Centered in middle
   - Save as: `splash_fullscreen.png`

2. **Optimize image:**
   - Use TinyPNG to compress
   - Target: < 500 KB

3. **Add to project:**
 - Copy to `Resources\Splash\`
   - Update .csproj

4. **Update .csproj:**

```xml
<MauiSplashScreen Include="Resources\Splash\splash_fullscreen.png" 
            Color="#1a1a2e" 
      BaseSize="2048,2048" />
```

5. **Rebuild and test**

---

## ?? Recommended Approach for AmbientSleeper

### **Step 1: Quick Test**

Update your current setup to see if larger size works:

```xml
<MauiSplashScreen Include="Resources\Splash\ambient_sleeper_background_and_small_icon2.png" 
                Color="#1a1a2e" 
       BaseSize="1536,1536" />
```

### **Step 2: If Pixelated**

Create a new 2048x2048 image:
- Dark blue background (#1a1a2e)
- Your icon/logo centered
- Optional: "Ambient Sleeper" text below

### **Step 3: Optimize & Deploy**

- Compress to < 1 MB
- Replace in Resources\Splash\
- Rebuild
- Test on devices

---

## ?? Size Comparison

| Size | Use Case | Quality | File Size |
|------|----------|---------|-----------|
| 512x512 | Small icon | Good for icon | ~100 KB |
| 1024x1024 | Medium splash | Acceptable | ~300 KB |
| 1536x1536 | Large splash | Good | ~600 KB |
| 2048x2048 | Full-screen | Excellent | ~1 MB |
| 2732x2732 | iPad Pro max | Perfect | ~2 MB |

**Recommendation:** 2048x2048 (best balance)

---

## ?? Current Configuration Update

I've updated your .csproj to:
```xml
<MauiSplashScreen Include="Resources\Splash\ambient_sleeper_background_and_small_icon2.png" 
 Color="#1a1a2e" 
 BaseSize="512,512"
   TintColor="#1a1a2e" />
```

**This removes `Resize="True"`** which was causing the small circle.

### **Next Steps:**

1. **Test current change:**
   ```powershell
   dotnet clean
   dotnet build -f net9.0-android
   ```

2. **If still too small, increase BaseSize:**
   ```xml
   BaseSize="1024,1024"
   ```
   or
   ```xml
   BaseSize="1536,1536"
   ```

3. **If pixelated, create new 2048x2048 image**

---

## ?? Tips

### **Testing Different Sizes:**

Try these BaseSize values progressively:
1. `512,512` (current - small)
2. `768,768` (medium)
3. `1024,1024` (large)
4. `1536,1536` (very large)
5. `2048,2048` (full-screen)

### **Avoiding Pixelation:**

- Keep BaseSize ? actual image size
- If you upscale, use 2x factor max
- Example: 512x512 image ? 1024x1024 BaseSize (okay)
- Example: 512x512 image ? 2048x2048 BaseSize (pixelated!)

### **File Size Management:**

- Compress all images with TinyPNG
- Target: < 500 KB for splash
- Larger BaseSize = need higher res source image

---

## ? What to Do Now

### **Immediate Action:**

1. **Try the change I just made:**
   - Removed `Resize="True"`
   - Added `TintColor="#1a1a2e"`

2. **Rebuild:**
```powershell
   dotnet clean
   dotnet build
   ```

3. **Test on device/emulator**

### **If Still Too Small:**

**Option A: Increase BaseSize (quick)**
```xml
BaseSize="1024,1024"
```

**Option B: Create new image (professional)**
- Design 2048x2048 splash image
- Place icon/logo in center
- Optimize to < 1 MB
- Replace current splash

---

## ?? Summary

**Question:** Can image fill entire screen instead of small circle?

**Answer:** YES! 

**Solutions:**
1. ? **Quick:** Increase `BaseSize` to 1024 or 1536
2. ? **Better:** Create 2048x2048 full-screen image
3. ? **Advanced:** Custom Android drawable

**My Change:** Removed `Resize="True"` which was forcing small size

**Next:** Test current change, then increase BaseSize if needed

---

**Created:** January 2025  
**Issue:** Small circular splash screen  
**Fix:** Increase BaseSize or create full-screen image  
**Status:** Initial fix applied, ready to test  
