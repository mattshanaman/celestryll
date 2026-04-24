# Quick Fix Guide - Remaining Localization Issues

## ?? CRITICAL FIX: Timer Notification

**File:** `ViewModels/TimerViewModel.cs`  
**Line:** ~155-160  
**Time:** 5 minutes

### Step 1: Add to AppResources.resx
```xml
<data name="Notification_TimerComplete_Title" xml:space="preserve">
  <value>AmbientSleeper</value>
</data>
<data name="Notification_TimerComplete_Description" xml:space="preserve">
  <value>Playback stopped. Timer completed.</value>
</data>
```

### Step 2: Update TimerViewModel.cs

**Add using statement:**
```csharp
using AmbientSleeper.Resources.Strings;
```

**Replace lines ~155-160:**
```csharp
// OLD:
var request = new NotificationRequest
{
    NotificationId = 2001,
    Title = "AmbientSleeper",
    Description = "Playback stopped. Timer completed.",
    // ...
};

// NEW:
var request = new NotificationRequest
{
    NotificationId = 2001,
    Title = AppResources.Notification_TimerComplete_Title,
    Description = AppResources.Notification_TimerComplete_Description,
    // ...
};
```

**Test:** Run app, set timer, verify notification appears with correct text.

---

## ?? MEDIUM FIX: Settings Diagnostics

**File:** `Views/SettingsPage.xaml.cs`  
**Lines:** 200-290  
**Time:** 20 minutes

### Resource Strings Needed

<details>
<summary>Click to expand full list of 20+ strings</summary>

```xml
<!-- Health Check -->
<data name="HealthCheck_Checking" xml:space="preserve">
  <value>Checking health...</value>
</data>
<data name="HealthCheck_AllHealthy" xml:space="preserve">
  <value>? All systems healthy</value>
</data>
<data name="HealthCheck_Passed" xml:space="preserve">
  <value>Health check passed!</value>
</data>
<data name="HealthCheck_IssuesDetected" xml:space="preserve">
  <value>? {0} issue(s) detected</value>
</data>
<data name="HealthCheck_ResultsTitle" xml:space="preserve">
  <value>Health Check Results</value>
</data>
<data name="HealthCheck_IssuesMessage" xml:space="preserve">
  <value>Issues detected:
• {0}

Some features may not work correctly.</value>
</data>
<data name="HealthCheck_Failed" xml:space="preserve">
  <value>? Health check failed</value>
</data>
<data name="HealthCheck_FailedMessage" xml:space="preserve">
  <value>Health check failed: {0}</value>
</data>

<!-- Error Report -->
<data name="ErrorReport_Title" xml:space="preserve">
  <value>Error Report</value>
</data>
<data name="ErrorReport_ServiceUnavailable" xml:space="preserve">
  <value>Error reporting service not available</value>
</data>
<data name="ErrorReport_NoErrors" xml:space="preserve">
  <value>No errors recorded.</value>
</data>
<data name="ErrorReport_CountFormat" xml:space="preserve">
  <value>Error Report ({0} errors)</value>
</data>
<data name="ErrorReport_ViewDetails" xml:space="preserve">
  <value>View Details</value>
</data>
<data name="ErrorReport_ShareReport" xml:space="preserve">
  <value>Share Report</value>
</data>
<data name="ErrorReport_ClearErrors" xml:space="preserve">
  <value>Clear Errors</value>
</data>
<data name="ErrorReport_SharedSuccess" xml:space="preserve">
  <value>Report shared successfully</value>
</data>
<data name="ErrorReport_ClearConfirmTitle" xml:space="preserve">
  <value>Clear Errors</value>
</data>
<data name="ErrorReport_ClearConfirmMessage" xml:space="preserve">
  <value>Are you sure you want to clear all recorded errors?</value>
</data>
<data name="ErrorReport_Cleared" xml:space="preserve">
  <value>Errors cleared</value>
</data>
<data name="ErrorReport_ViewFailed" xml:space="preserve">
  <value>Failed to view error report: {0}</value>
</data>

<!-- Common Buttons -->
<data name="Yes" xml:space="preserve">
  <value>Yes</value>
</data>
<data name="No" xml:space="preserve">
  <value>No</value>
</data>
<data name="Error" xml:space="preserve">
  <value>Error</value>
</data>
```
</details>

### Code Changes

**Add using statement if not present:**
```csharp
using AmbientSleeper.Resources.Strings;
```

**Replace hardcoded strings throughout OnCheckHealthClicked and OnViewErrorReportClicked methods.**

See `COMPREHENSIVE_LOCALIZATION_FINAL_AUDIT.md` for detailed before/after code.

---

## ?? LOW FIX: Navigation Errors

**File:** `Views/PlaybackPage.xaml.cs`  
**Lines:** 42, 57  
**Time:** 5 minutes

### Step 1: Add to AppResources.resx
```xml
<data name="NavigationError_Title" xml:space="preserve">
  <value>Navigation Error</value>
</data>
<data name="NavigationError_Settings" xml:space="preserve">
  <value>Could not open Settings page.</value>
</data>
```

### Step 2: Update PlaybackPage.xaml.cs

**Replace line ~42:**
```csharp
// OLD:
await DisplayAlert("Navigation error", "Could not open Settings page.", "OK");

// NEW:
await DisplayAlert(AppResources.NavigationError_Title, 
    AppResources.NavigationError_Settings, 
    AppResources.Ok);
```

**Replace line ~57:**
```csharp
// OLD:
await DisplayAlert("Navigation error", ex.Message, "OK");

// NEW:
await DisplayAlert(AppResources.NavigationError_Title, 
    ex.Message, 
    AppResources.Ok);
```

---

## Testing Checklist

After each fix:

### Critical Fix
- [ ] Build succeeds
- [ ] Set timer for 1 minute
- [ ] Wait for completion
- [ ] Verify notification appears
- [ ] Check notification text is correct

### Medium Fix
- [ ] Build succeeds
- [ ] Navigate to Settings ? Diagnostics
- [ ] Click "Check Health"
- [ ] Verify all messages use resource strings
- [ ] Click "View Error Report"
- [ ] Test all dialogs and buttons

### Low Fix
- [ ] Build succeeds
- [ ] Test navigation to Settings
- [ ] Verify error handling works

---

## Order of Implementation

**Recommended:**
1. ? Critical fix first (notifications are user-facing)
2. ? Low fix second (quick win, low risk)
3. ? Medium fix last (takes more time, admin feature)

**OR**

Do all at once:
1. Add all resource strings to AppResources.resx
2. Update all three files
3. Build and test everything

---

## Build Command

After changes:
```bash
dotnet build
```

If errors:
- Check AppResources.Designer.cs was regenerated
- Verify all resource string names match exactly
- Check using statements are present

---

**Total Time:** 30-45 minutes  
**Files to Edit:** 4 (AppResources.resx + 3 C# files)  
**Risk:** Low (isolated changes)  
**Impact:** High (completes 100% localization)
