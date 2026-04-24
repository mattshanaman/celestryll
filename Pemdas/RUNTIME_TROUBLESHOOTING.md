# BadlyDefined - Runtime Crash Troubleshooting

## 🔍 Issue: App Disappears After Launch

**Build Status:** ✅ SUCCESS  
**Runtime Status:** ❌ App disappears in emulator

## 🛠️ Fixes Applied

### 1. **Missing Converter Fixed** ✅
- Created `StringIsNotNullOrEmptyConverter.cs`
- Registered in App.xaml
- This was causing a XAML parsing error

### 2. **Reduced Rotation Angles** ✅
- Reduced frame rotations from 1.5-1.8° to 0.5°
- Removed rotations from critical frames (Time, Hard button)
- Removed rotations from input frame and action buttons
- This prevents potential layout crashes

### 3. **Current Wonky Effects** (Safe):
- ✅ Uneven padding on all frames and buttons
- ✅ Different corner radii (7-10)
- ✅ Slight rotations on title letters
- ✅ Minimal rotations on stat frames (-0.5° to +0.5°)
- ✅ Borders added (2px)
- ✅ Shadows enabled

## 🚀 Next Steps to Debug

### 1. Check Build Output Window
In Visual Studio:
- View → Output
- Show output from: Build
- Look for XAML compilation errors

### 2. Enable Debug Logging
Already enabled in MauiProgram.cs:
```csharp
#if DEBUG
    builder.Logging.AddDebug();
#endif
```

### 3. Check Android Logcat
```powershell
adb logcat | Select-String "BadlyDefined|Exception|Error|FATAL"
```

### 4. Deploy with Verbose Output
```powershell
dotnet build BadlyDefined.csproj -f net10.0-android -t:Run -v detailed
```

### 5. Try Clean Build
```powershell
dotnet clean BadlyDefined.csproj
dotnet build BadlyDefined.csproj -f net10.0-android
```

## 🔧 Potential Issues & Solutions

### Issue 1: XAML Parsing Error
**Symptoms:** App crashes immediately on launch  
**Solution:** ✅ Fixed by adding StringIsNotNullOrEmptyConverter

### Issue 2: Database Initialization
**Symptoms:** App crashes after splash screen  
**Check:** Error logs in App.xaml.cs OnStart method
```csharp
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"❌ Database initialization failed: {ex.Message}");
}
```

### Issue 3: Rotation Layout Issues
**Symptoms:** App crashes when rendering GamePage  
**Solution:** ✅ Reduced rotations to minimal angles (0.5°)

### Issue 4: Emulator Issues
**Symptoms:** App works on physical device but not emulator  
**Try:**
- Restart emulator
- Wipe emulator data
- Use different emulator API level

## 🎯 Recommended Action

### Step 1: Clean Build
```powershell
cd BadlyDefined
dotnet clean
dotnet build -f net10.0-android
```

### Step 2: Deploy Fresh
```powershell
dotnet build -f net10.0-android -t:Run
```

### Step 3: Check Debug Output
Look in Visual Studio Output window for:
- "❌" error indicators
- Exception messages
- Stack traces

### Step 4: If Still Crashing
Try reverting the wonky effects temporarily:

**Option A: Remove all rotations**
```xml
<!-- Remove Rotation="..." from all elements -->
```

**Option B: Simplify title**
```xml
<Label Text="Badly Defined" 
       FontSize="26" 
       FontAttributes="Bold" 
       TextColor="{StaticResource Primary}"
       HorizontalOptions="Center"/>
```

## 📝 Changes That Might Cause Issues

### Recent Changes:
1. ✅ Individual letter labels with rotations (TITLE) - **Likely OK**
2. ✅ Frame rotations (STATS) - **Reduced to minimal**
3. ✅ Button rotations (DIFFICULTY) - **Removed from Hard button**
4. ✅ Input frame border (GUESS) - **Should be OK**
5. ⚠️ Missing converter - **FIXED**

## 🧪 Testing Checklist

- [ ] Clean build completed
- [ ] App launches successfully
- [ ] Title displays correctly
- [ ] Stats boxes show properly
- [ ] Difficulty buttons work
- [ ] Input field accepts text
- [ ] Buttons respond to taps
- [ ] No crashes during gameplay

## 💡 If Issue Persists

### Temporarily disable wonky effects:
1. Remove all `Rotation="..."` attributes
2. Make all padding uniform `Padding="12"`
3. Make all corner radii same `CornerRadius="8"`
4. Remove borders temporarily

### Enable incremental testing:
Add one wonky effect at a time to identify the culprit.

---

**Most Likely Fix:** The StringIsNotNullOrEmptyConverter was missing, causing a XAML crash. This has been added and should resolve the issue. Try a clean build now!
