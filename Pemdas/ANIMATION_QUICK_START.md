# ?? Quick Start: Adding Animations to Your XAML Pages

## Step 1: Import the Helper

Add to your code-behind (.xaml.cs):

```csharp
using Pemdas.Helpers;
```

## Step 2: Name Your UI Elements

In your XAML, add `x:Name` to elements you want to animate:

```xml
<Button x:Name="SubmitButton" 
        Text="Submit" 
        Clicked="OnSubmitClicked"/>

<Label x:Name="FeedbackLabel" 
       Text="{Binding FeedbackMessage}"
       IsVisible="{Binding ShowFeedback}"/>

<Entry x:Name="AnswerEntry"
       Text="{Binding UserInput}"/>
```

## Step 3: Add Animations to Event Handlers

### Example: Submit Button with Success/Error Feedback

```csharp
private async void OnSubmitClicked(object sender, EventArgs e)
{
    // Press animation on click
    await SubmitButton.AnimatePress();
    
    // Execute command
    await ViewModel.SubmitAnswerCommand.ExecuteAsync(null);
    
    // Show feedback with animation
    if (ViewModel.IsCorrect)
    {
        await FeedbackLabel.AnimateSuccessWithColor(Colors.Transparent);
        await AnswerEntry.AnimateSuccess();
    }
    else
    {
        await FeedbackLabel.AnimateErrorWithColor(Colors.Transparent);
        await AnswerEntry.AnimateError();
    }
}
```

### Example: Hint Button with Pulse

```csharp
private async void OnHintClicked(object sender, EventArgs e)
{
    await HintButton.AnimatePress();
    await ViewModel.UseHintCommand.ExecuteAsync(null);
    
    if (ViewModel.ShowHint)
    {
        await HintLabel.FadeIn();
        await HintLabel.AnimatePulse(2);
    }
}
```

### Example: Animated Number Counter

```csharp
private async void OnScoreUpdated(int oldScore, int newScore)
{
    await ScoreLabel.AnimateNumberIncrement(oldScore, newScore, 1000);
    await ScoreLabel.AnimateBounce();
}
```

### Example: Streak Celebration

```csharp
partial void OnCurrentStreakChanged(int value)
{
    if (value > 0 && value % 7 == 0) // Weekly milestone
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await StreakIcon.AnimateStreakCelebration();
            await StreakLabel.AnimateCelebration();
        });
    }
}
```

## Step 4: Page Load Animations

```csharp
protected override async void OnAppearing()
{
    base.OnAppearing();
    
    // Stagger animations for smooth entrance
    await Task.Delay(100);
    await TitleLabel.SlideInFromLeft(300);
    
    await Task.Delay(100);
    await PuzzleCard.FadeIn(300);
    
    await Task.Delay(100);
    await ButtonPanel.SlideInFromRight(300);
}
```

## Step 5: Combine with Haptic Feedback

```csharp
private async void OnDigitTapped(object sender, EventArgs e)
{
    var button = (Button)sender;
    
    // Haptic feedback
    _feedbackService.LightTap();
    
    // Visual animation
    await button.AnimatePress();
    
    // Update input
    ViewModel.AddDigitCommand.Execute(button.Text);
}
```

## Complete GamePage Example

```csharp
using Pemdas.Helpers;
using Pemdas.ViewModels;

namespace Pemdas.Pages;

public partial class GamePage : ContentPage
{
    private readonly GameViewModel _viewModel;
    private readonly IFeedbackService _feedbackService;

    public GamePage(GameViewModel viewModel, IFeedbackService feedbackService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _feedbackService = feedbackService;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
        
        // Entrance animations
        await AnimatePageLoad();
    }

    private async Task AnimatePageLoad()
    {
        // Stagger entrance for professional feel
        await PuzzleLabel.FadeIn(300);
        await Task.Delay(100);
        await AnswerEntry.SlideInFromLeft(300);
        await Task.Delay(100);
        await ButtonGrid.FadeIn(300);
    }

    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        await SubmitButton.AnimatePress();
        _feedbackService.MediumImpact();
        
        await _viewModel.SubmitAnswerCommand.ExecuteAsync(null);
        
        if (_viewModel.ShowFeedback)
        {
            if (_viewModel.PuzzleCompleted)
            {
                await FeedbackLabel.AnimateSuccessWithColor(Colors.White);
                await ConfettiAnimation();
            }
            else
            {
                await FeedbackLabel.AnimateErrorWithColor(Colors.White);
                await AnswerEntry.AnimateError();
            }
        }
    }

    private async void OnHintClicked(object sender, EventArgs e)
    {
        await HintButton.AnimatePress();
        _feedbackService.LightTap();
        
        await _viewModel.UseHintCommand.ExecuteAsync(null);
        
        if (_viewModel.ShowHint)
        {
            await HintLabel.FadeIn();
            await HintLabel.AnimatePulse(2);
        }
    }

    private async void OnDigitClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        _feedbackService.LightTap();
        await button.AnimatePress();
        _viewModel.AddDigitCommand.Execute(button.Text);
    }

    private async void OnClearClicked(object sender, EventArgs e)
    {
        _feedbackService.MediumImpact();
        await ClearButton.AnimatePress();
        await AnswerEntry.AnimateError();
        _viewModel.ClearInputCommand.Execute(null);
    }

    private async Task ConfettiAnimation()
    {
        // Celebrate success with multiple effects
        await Task.WhenAll(
            StreakLabel.AnimateStreakCelebration(),
            PointsLabel.AnimateBounce(),
            FeedbackLabel.AnimateCelebration()
        );
    }
}
```

## Animation Timing Tips

### Quick Actions (< 150ms)
- Button presses
- Digit entry
- Light taps

```csharp
await button.AnimatePress(); // 100ms total
```

### Standard Actions (150-400ms)
- Fades
- Slides
- Success/Error feedback

```csharp
await element.FadeIn(300);
await element.SlideInFromLeft(300);
```

### Celebrations (500-1000ms)
- Streaks
- Achievements
- Level completions

```csharp
await element.AnimateStreakCelebration(); // 700ms
await label.AnimateNumberIncrement(0, 100, 1000);
```

## Common Patterns

### Pattern 1: Button Tap Feedback
```csharp
private async void OnButtonClicked(object sender, EventArgs e)
{
    var button = (Button)sender;
    _feedbackService.LightTap();           // Haptic
    await button.AnimatePress();            // Visual
    // Execute action
}
```

### Pattern 2: Success Feedback
```csharp
private async Task ShowSuccess()
{
    await _feedbackService.PlaySuccessFeedback();  // Audio + Haptic
    await element.AnimateSuccessWithColor(originalColor);  // Visual
}
```

### Pattern 3: Error Feedback
```csharp
private async Task ShowError()
{
    await _feedbackService.PlayErrorFeedback();  // Audio + Haptic
    await element.AnimateErrorWithColor(originalColor);  // Visual
}
```

### Pattern 4: Progressive Reveal
```csharp
private async Task RevealElements()
{
    foreach (var element in elementsToReveal)
    {
        await element.FadeIn(200);
        await Task.Delay(50);  // Stagger delay
    }
}
```

## Best Practices

1. **Always use `await`** - Don't fire-and-forget animations
2. **Stagger animations** - Add small delays between sequential animations
3. **Combine feedback types** - Haptic + Visual or Audio + Visual
4. **Test on device** - Animations may look different on real hardware
5. **Handle exceptions** - Animations can fail on low-end devices
6. **Use MainThread** - Ensure animations run on UI thread

```csharp
MainThread.BeginInvokeOnMainThread(async () =>
{
    await element.AnimateSuccess();
});
```

## Platform-Specific Notes

### iOS
- Haptics work best (Taptic Engine)
- Smooth 60fps animations
- Respects "Reduce Motion" setting

### Android
- Vibration API for haptics
- May vary by device
- Some devices don't support haptics

### Windows/macOS
- Limited haptic support
- Focus on visual animations
- Audio feedback works well

---

**Quick Start Complete! ??**

Your app now has professional-grade animations and feedback!
