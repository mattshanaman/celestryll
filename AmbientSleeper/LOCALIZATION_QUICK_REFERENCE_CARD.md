# ?? Localization Quick Reference Card

**Print this page or keep it open while working**

---

## ?? The Facts

```
STATUS:     88% Complete
FILES:      3 need fixes
TIME:       30-45 minutes
RISK:       LOW
PRIORITY:   HIGH
```

---

## ?? Critical Fix (5 min)

**File:** `ViewModels/TimerViewModel.cs`  
**Line:** ~155-160

### Add to AppResources.resx:
```xml
<data name="Notification_TimerComplete_Title" xml:space="preserve">
  <value>AmbientSleeper</value>
</data>
<data name="Notification_TimerComplete_Description" xml:space="preserve">
  <value>Playback stopped. Timer completed.</value>
</data>
```

### Change in code:
```csharp
// Add using:
using AmbientSleeper.Resources.Strings;

// Change from:
Title = "AmbientSleeper",
Description = "Playback stopped. Timer completed.",

// To:
Title = AppResources.Notification_TimerComplete_Title,
Description = AppResources.Notification_TimerComplete_Description,
```

---

## ?? Low Fix (5 min)

**File:** `Views/PlaybackPage.xaml.cs`  
**Lines:** 42, 57

### Add to AppResources.resx:
```xml
<data name="NavigationError_Title" xml:space="preserve">
  <value>Navigation Error</value>
</data>
<data name="NavigationError_Settings" xml:space="preserve">
  <value>Could not open Settings page.</value>
</data>
```

### Change in code (line 42):
```csharp
// From:
await DisplayAlert("Navigation error", "Could not open Settings page.", "OK");

// To:
await DisplayAlert(AppResources.NavigationError_Title, 
    AppResources.NavigationError_Settings, 
    AppResources.Ok);
```

### Change in code (line 57):
```csharp
// From:
await DisplayAlert("Navigation error", ex.Message, "OK");

// To:
await DisplayAlert(AppResources.NavigationError_Title, 
    ex.Message, 
    AppResources.Ok);
```

---

## ?? Medium Fix (20 min)

**File:** `Views/SettingsPage.xaml.cs`  
**Lines:** 200-290

**Too many strings to list here.**  
See `LOCALIZATION_QUICK_FIX_GUIDE.md` for complete list.

---

## ? Quick Test

After fixes:

```bash
# Build
dotnet build

# If successful:
1. Set 1-minute timer
2. Wait for completion
3. Check notification text
4. Done! ?
```

---

## ?? Full Documentation

```
LOCALIZATION_AUDIT_START_HERE.md          ? Start
LOCALIZATION_QUICK_FIX_GUIDE.md          ? Implement
LOCALIZATION_COMPLETION_CHECKLIST.md     ? Track
```

---

## ?? Quick Commands

```bash
# Build
dotnet build

# Clean build
dotnet clean
dotnet build

# Rebuild (if AppResources.Designer.cs not updating)
# Just save AppResources.resx in Visual Studio
# Or manually rebuild the project
```

---

## ?? Common Issues

**Issue:** AppResources.Notification_TimerComplete_Title not found  
**Fix:** Build the project to regenerate AppResources.Designer.cs

**Issue:** Changes not taking effect  
**Fix:** Clean and rebuild the solution

**Issue:** Still seeing English  
**Fix:** Verify exact resource string names (case sensitive)

---

## ?? Progress Tracker

```
? Add timer notification strings to AppResources.resx
? Update TimerViewModel.cs
? Build and test timer notification
? Add navigation error strings to AppResources.resx
? Update PlaybackPage.xaml.cs (line 42)
? Update PlaybackPage.xaml.cs (line 57)
? Build and test navigation
? (Optional) Add diagnostic strings
? (Optional) Update SettingsPage.xaml.cs
? Final build
? Final test
? ? DONE - 100% Localized!
```

---

## ?? Minimum Path (10 min)

1. Add 4 resource strings (timer + navigation)
2. Update 2 files (TimerViewModel.cs, PlaybackPage.xaml.cs)
3. Build and test
4. **Result: 92% complete**

---

## ?? Complete Path (30 min)

1. Do minimum path (10 min)
2. Add ~23 diagnostic strings (5 min)
3. Update SettingsPage.xaml.cs (15 min)
4. Build and test
5. **Result: 100% complete**

---

## ?? Tips

? Copy-paste resource strings exactly  
? Build after each change to catch errors  
? Check AppResources.Designer.cs regenerates  
? Test notifications after timer fix  
? Use Ctrl+F to find exact line numbers  

---

## ?? Need Help?

**Full guide:** `LOCALIZATION_QUICK_FIX_GUIDE.md`  
**All details:** `COMPREHENSIVE_LOCALIZATION_FINAL_AUDIT.md`  
**Business case:** `LOCALIZATION_EXECUTIVE_SUMMARY.md`

---

**Time to 100%: 30-45 minutes**  
**You've got this! ??**
