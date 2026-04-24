# Localization Configuration Guide

## Overview
This document provides complete instructions for configuring and managing multiple language support in the PEMDAS application.

## ? Already Configured

### 1. Project File Setup
The `Pemdas.csproj` file is configured with:
```xml
<ItemGroup>
    <EmbeddedResource Update="Resources\Localization\AppResources.resx">
        <Generator>ResXFileCodeGenerator</Generator>
        <LastGenOutput>AppResources.Designer.cs</LastGenOutput>
    </EmbeddedResource>
    <Compile Update="Resources\Localization\AppResources.Designer.cs">
        <DesignTime>True</DesignTime>
        <AutoGen>True</AutoGen>
        <DependentUpon>AppResources.resx</DependentUpon>
    </Compile>
</ItemGroup>
```

### 2. Services Created
- ? `LocalizationService.cs` - Manages culture selection
- ? `LocalizationResourceManager.cs` - Runtime resource management

### 3. Supported Languages
The application is configured to support:
- ???? English (en) - **Default**
- ???? Spanish (es)
- ???? French (fr)
- ???? German (de)
- ???? Italian (it)
- ???? Portuguese (pt)
- ???? Japanese (ja)
- ???? Chinese Simplified (zh-Hans)
- ???? Korean (ko)
- ???? Arabic (ar)
- ???? Russian (ru)
- ???? Hindi (hi)

## ?? Required Steps to Complete Setup

### Step 1: Create AppResources.resx (Default English)

**Location:** `Resources/Localization/AppResources.resx`

This is the **default** resource file containing English strings. You need to create this file in Visual Studio:

1. Right-click on `Resources/Localization` folder
2. Select **Add ? New Item**
3. Choose **Resources File (.resx)**
4. Name it `AppResources.resx`
5. Add all string resources (see template below)

### Step 2: Create Language-Specific Resource Files

For each additional language, create a resource file with the culture code:

**Format:** `AppResources.[culture].resx`

Examples:
- `AppResources.es.resx` (Spanish)
- `AppResources.fr.resx` (French)
- `AppResources.de.resx` (German)
- `AppResources.ja.resx` (Japanese)
- `AppResources.zh-Hans.resx` (Chinese Simplified)

**Steps:**
1. Right-click `Resources/Localization` folder
2. Select **Add ? New Item ? Resources File**
3. Name it with the culture code (e.g., `AppResources.es.resx`)
4. Copy all keys from `AppResources.resx`
5. Translate only the **values**, keep the **keys** identical

### Step 3: Generate Designer File

After creating `AppResources.resx`:

1. Right-click on `AppResources.resx` in Solution Explorer
2. Select **Properties**
3. Set **Custom Tool** to `PublicResXFileCodeGenerator`
4. Set **Build Action** to `Embedded Resource`
5. Save and close
6. Right-click `AppResources.resx` ? **Run Custom Tool**

This generates `AppResources.Designer.cs` automatically.

## ?? String Resource Template

Here are the key string resources to include in your `AppResources.resx`:

### Application
```
AppTitle = PEMDAS - Daily Math Challenge
```

### Navigation
```
TabDailyChallenge = Daily Challenge
TabProfile = Profile
```

### Game Page
```
Streak = Streak
Hints = Hints
Time = Time
Mode = Mode
Difficulty = Difficulty
EnterAnswer = Enter your answer...
ButtonHint = ?? Hint
ButtonSubmit = ? Submit
ButtonShare = ?? Share Result
ButtonClear = Clear
CorrectAnswer = ?? Correct! You earned {0} points!
WrongAnswer = ? Not quite right. Try again!
```

### Profile Page
```
YourStats = Your Stats
CurrentStreak = Current Streak
BestStreak = Best Streak
PuzzlesSolved = Puzzles Solved
TotalPoints = Total Points
HintTokensAvailable = Hint Tokens Available
PremiumMember = Premium Member
GoPremium = Go Premium!
SubscribeButton = Subscribe Now - $2.99/month
ViewArchive = ?? View Puzzle Archive
```

### Common
```
Success = Success
Error = Error
OK = OK
```

### Difficulty Levels
```
Easy = Easy
Medium = Medium
Hard = Hard
Creative = Creative
Tricky = Tricky
Speed = Speed
Boss = Boss
```

### Game Modes
```
SolveIt = Solve It
BuildIt = Build It
```

## ?? Using Localization in Code

### ViewModels
```csharp
using Pemdas.Resources.Localization;

public class GameViewModel : BaseViewModel
{
    public void SetupMessages()
    {
        // Access localized strings
        FeedbackMessage = AppResources.CorrectAnswer;
        ErrorMessage = AppResources.WrongAnswer;
        
        // With formatting
        var points = 100;
        FeedbackMessage = string.Format(AppResources.CorrectAnswer, points);
    }
}
```

### XAML Binding
```xaml
<!-- Add namespace -->
xmlns:res="clr-namespace:Pemdas.Resources.Localization"

<!-- Static binding -->
<Label Text="{x:Static res:AppResources.AppTitle}" />

<!-- In ViewModels, expose as properties -->
<Label Text="{Binding LocalizedTitle}" />
```

## ?? Runtime Language Switching

### Register Service in MauiProgram.cs
```csharp
builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
```

### Switch Language in Code
```csharp
public class SettingsViewModel
{
    private readonly ILocalizationService _localizationService;
    
    public SettingsViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
    }
    
    [RelayCommand]
    private void ChangeLanguage(string cultureName)
    {
        var culture = new CultureInfo(cultureName);
        _localizationService.SetCulture(culture);
        
        // Restart the app for full effect
        Application.Current.Quit();
    }
}
```

### Get Supported Languages
```csharp
var supportedLanguages = _localizationService.GetSupportedCultures();
foreach (var culture in supportedLanguages)
{
    Console.WriteLine($"{culture.DisplayName} ({culture.Name})");
}
```

## ?? Creating a Language Selector Page

### Example ViewModel
```csharp
public partial class LanguageSelectionViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;
    
    [ObservableProperty]
    private ObservableCollection<CultureInfo> supportedLanguages;
    
    [ObservableProperty]
    private CultureInfo selectedLanguage;
    
    public LanguageSelectionViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        SupportedLanguages = new ObservableCollection<CultureInfo>(
            _localizationService.GetSupportedCultures()
        );
        SelectedLanguage = _localizationService.CurrentCulture;
    }
    
    [RelayCommand]
    private async Task ApplyLanguage()
    {
        _localizationService.SetCulture(SelectedLanguage);
        
        await Application.Current.MainPage.DisplayAlert(
            AppResources.Success,
            "Language changed. Restart the app to see changes.",
            AppResources.OK
        );
    }
}
```

### Example XAML
```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:res="clr-namespace:Pemdas.Resources.Localization"
             Title="{x:Static res:AppResources.SelectLanguage}">
    
    <VerticalStackLayout Padding="20">
        <Label Text="{x:Static res:AppResources.ChooseLanguage}" 
               FontSize="20" 
               Margin="0,0,0,20"/>
        
        <Picker ItemsSource="{Binding SupportedLanguages}"
                SelectedItem="{Binding SelectedLanguage}"
                ItemDisplayBinding="{Binding DisplayName}"/>
        
        <Button Text="{x:Static res:AppResources.Apply}"
                Command="{Binding ApplyLanguageCommand}"
                Margin="0,20,0,0"/>
    </VerticalStackLayout>
</ContentPage>
```

## ?? Platform-Specific Considerations

### Android
No special configuration needed. Language follows system settings automatically.

### iOS
Add supported languages to `Info.plist`:
```xml
<key>CFBundleLocalizations</key>
<array>
    <string>en</string>
    <string>es</string>
    <string>fr</string>
    <string>de</string>
    <string>ja</string>
</array>
```

### Windows
No special configuration needed.

## ?? Testing Localization

### Test on Emulator/Simulator
1. Change device language in settings
2. Restart your app
3. Verify strings are translated

### Test Language Switching
```csharp
// In your test code
var localizationService = new LocalizationService();
localizationService.SetCulture(new CultureInfo("es")); // Spanish
Console.WriteLine(AppResources.AppTitle); // Should show Spanish
```

## ?? Best Practices

### 1. Key Naming Convention
- Use PascalCase for keys
- Make keys descriptive
- Group related keys with prefixes (e.g., `Button`, `Message`, `Error`)

### 2. Avoid Hardcoded Strings
? Bad:
```csharp
ButtonText = "Click Me";
```

? Good:
```csharp
ButtonText = AppResources.ButtonSubmit;
```

### 3. Use String Formatting
For dynamic content:
```csharp
// In resource file
CorrectAnswer = ?? Correct! You earned {0} points!

// In code
var message = string.Format(AppResources.CorrectAnswer, points);
```

### 4. Comments in Resource Files
Add comments to provide context for translators:
```xml
<data name="CorrectAnswer" xml:space="preserve">
    <value>?? Correct! You earned {0} points!</value>
    <comment>{0} will be replaced with the number of points earned</comment>
</data>
```

### 5. Emoji and Special Characters
- Test emojis on all platforms
- Some emojis may render differently
- Consider using platform-specific alternatives if needed

## ?? Troubleshooting

### Issue: AppResources not found
**Solution:** 
1. Ensure `AppResources.resx` exists
2. Run Custom Tool to generate Designer file
3. Rebuild the project

### Issue: Translations not appearing
**Solution:**
1. Check culture code is correct (e.g., `es`, not `es-ES`)
2. Ensure Build Action is set to `Embedded Resource`
3. Verify keys match exactly between language files

### Issue: App crashes on startup
**Solution:**
1. Check AppResources.Designer.cs is generated
2. Ensure namespace is correct: `Pemdas.Resources.Localization`
3. Verify no syntax errors in .resx files

## ?? Distribution

When publishing:
1. All `.resx` files are embedded automatically
2. Satellite assemblies are created for each language
3. No special distribution steps required
4. App size increases slightly with each language (~5-10KB per language)

## ?? Quick Start Checklist

- [ ] Create `Resources/Localization/AppResources.resx`
- [ ] Add all string resources with English values
- [ ] Generate Designer file (Run Custom Tool)
- [ ] Create language-specific `.resx` files
- [ ] Translate values in each language file
- [ ] Register `LocalizationService` in `MauiProgram.cs`
- [ ] Update ViewModels to use `AppResources`
- [ ] Test with different languages
- [ ] Add language selector UI (optional)

## ?? Additional Resources

- [Microsoft Docs - Localization](https://docs.microsoft.com/dotnet/maui/fundamentals/localization)
- [.NET Resource Files](https://docs.microsoft.com/dotnet/core/extensions/resources)
- [CultureInfo Class](https://docs.microsoft.com/dotnet/api/system.globalization.cultureinfo)

---

**Note:** The infrastructure is fully configured. You just need to create the `.resx` files and add your translations!
