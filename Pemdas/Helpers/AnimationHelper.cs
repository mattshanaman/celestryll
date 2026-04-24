namespace Pemdas.Helpers;

public static class AnimationHelper
{
    // Success animations
    public static async Task AnimateSuccess(this VisualElement element)
    {
        try
        {
            await element.ScaleTo(1.2, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in success animation: {ex.Message}");
        }
    }

    public static async Task AnimateSuccessWithColor(this VisualElement element, Color originalColor)
    {
        try
        {
            var successColor = Color.FromRgba(76, 175, 80, 255); // Material Green
            
            element.BackgroundColor = successColor;
            await element.ScaleTo(1.1, 100, Easing.CubicOut);
            await element.ScaleTo(1.0, 100, Easing.CubicIn);
            
            await Task.Delay(500);
            element.BackgroundColor = originalColor;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in success color animation: {ex.Message}");
        }
    }

    // Error animations
    public static async Task AnimateError(this VisualElement element)
    {
        try
        {
            await element.TranslateTo(-10, 0, 50);
            await element.TranslateTo(10, 0, 50);
            await element.TranslateTo(-10, 0, 50);
            await element.TranslateTo(10, 0, 50);
            await element.TranslateTo(0, 0, 50);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in error animation: {ex.Message}");
        }
    }

    public static async Task AnimateErrorWithColor(this VisualElement element, Color originalColor)
    {
        try
        {
            var errorColor = Color.FromRgba(244, 67, 54, 255); // Material Red
            
            element.BackgroundColor = errorColor;
            await AnimateError(element);
            
            await Task.Delay(500);
            element.BackgroundColor = originalColor;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in error color animation: {ex.Message}");
        }
    }

    // Button press animation
    public static async Task AnimatePress(this VisualElement element)
    {
        try
        {
            await element.ScaleTo(0.95, 50, Easing.CubicOut);
            await element.ScaleTo(1.0, 50, Easing.CubicIn);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in press animation: {ex.Message}");
        }
    }

    // Fade animations
    public static async Task FadeIn(this VisualElement element, uint duration = 300)
    {
        try
        {
            element.Opacity = 0;
            element.IsVisible = true;
            await element.FadeTo(1, duration, Easing.CubicInOut);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in fade in animation: {ex.Message}");
        }
    }

    public static async Task FadeOut(this VisualElement element, uint duration = 300)
    {
        try
        {
            await element.FadeTo(0, duration, Easing.CubicInOut);
            element.IsVisible = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in fade out animation: {ex.Message}");
        }
    }

    // Bounce animation
    public static async Task AnimateBounce(this VisualElement element)
    {
        try
        {
            await element.ScaleTo(1.3, 100, Easing.BounceOut);
            await element.ScaleTo(1.0, 100, Easing.BounceIn);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in bounce animation: {ex.Message}");
        }
    }

    // Pulse animation for hints/notifications
    public static async Task AnimatePulse(this VisualElement element, int count = 3)
    {
        try
        {
            for (int i = 0; i < count; i++)
            {
                await element.ScaleTo(1.1, 200, Easing.CubicInOut);
                await element.ScaleTo(1.0, 200, Easing.CubicInOut);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in pulse animation: {ex.Message}");
        }
    }

    // Streak celebration animation
    public static async Task AnimateStreakCelebration(this VisualElement element)
    {
        try
        {
            await element.RotateTo(360, 500, Easing.SpringOut);
            element.Rotation = 0;
            await element.ScaleTo(1.3, 200, Easing.BounceOut);
            await element.ScaleTo(1.0, 200, Easing.BounceIn);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in streak celebration animation: {ex.Message}");
        }
    }

    // Number increment animation
    public static async Task AnimateNumberIncrement(this Label label, int from, int to, uint duration = 1000)
    {
        try
        {
            var steps = Math.Abs(to - from);
            var stepDuration = duration / Math.Max(steps, 1);
            var increment = to > from ? 1 : -1;

            for (int i = from; (increment > 0 ? i <= to : i >= to); i += increment)
            {
                label.Text = i.ToString();
                await Task.Delay((int)stepDuration);
            }

            label.Text = to.ToString();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in number increment animation: {ex.Message}");
        }
    }

    // Slide in from side
    public static async Task SlideInFromLeft(this VisualElement element, uint duration = 300)
    {
        try
        {
            element.TranslationX = -element.Width;
            element.IsVisible = true;
            await element.TranslateTo(0, 0, duration, Easing.CubicOut);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in slide in animation: {ex.Message}");
        }
    }

    public static async Task SlideInFromRight(this VisualElement element, uint duration = 300)
    {
        try
        {
            element.TranslationX = element.Width;
            element.IsVisible = true;
            await element.TranslateTo(0, 0, duration, Easing.CubicOut);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in slide in animation: {ex.Message}");
        }
    }

    // Countdown urgency animation
    public static async Task AnimateUrgency(this VisualElement element)
    {
        try
        {
            var warningColor = Color.FromRgba(255, 152, 0, 255); // Material Orange
            element.BackgroundColor = warningColor;
            
            await element.ScaleTo(1.05, 100);
            await element.ScaleTo(1.0, 100);
            
            // Restore color after a moment
            await Task.Delay(200);
            element.BackgroundColor = Colors.Transparent;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in urgency animation: {ex.Message}");
        }
    }

    // Confetti-style celebration (text color flash)
    public static async Task AnimateCelebration(this Label label)
    {
        try
        {
            var colors = new[]
            {
                Color.FromRgba(255, 193, 7, 255),   // Amber
                Color.FromRgba(76, 175, 80, 255),   // Green
                Color.FromRgba(33, 150, 243, 255),  // Blue
                Color.FromRgba(156, 39, 176, 255),  // Purple
            };

            var originalColor = label.TextColor;

            foreach (var color in colors)
            {
                label.TextColor = color;
                await label.ScaleTo(1.2, 100);
                await label.ScaleTo(1.0, 100);
            }

            label.TextColor = originalColor;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in celebration animation: {ex.Message}");
        }
    }
}
