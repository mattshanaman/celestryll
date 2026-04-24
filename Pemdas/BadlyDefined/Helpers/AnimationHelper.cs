using System.Diagnostics;

namespace BadlyDefined.Helpers;

/// <summary>
/// Provides smooth animations for UI elements
/// </summary>
public static class AnimationHelper
{
    /// <summary>
    /// Pulse animation for success
    /// </summary>
    public static async Task PulseSuccess(View view)
    {
        try
        {
            await view.ScaleToAsync(1.2, 100);
            await view.ScaleToAsync(1.0, 100);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Shake animation for error
    /// </summary>
    public static async Task Shake(View view)
    {
        try
        {
            await view.TranslateToAsync(-15, 0, 50);
            await view.TranslateToAsync(15, 0, 50);
            await view.TranslateToAsync(-10, 0, 50);
            await view.TranslateToAsync(10, 0, 50);
            await view.TranslateToAsync(0, 0, 50);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Fade in animation
    /// </summary>
    public static async Task FadeIn(View view, uint duration = 250)
    {
        try
        {
            view.Opacity = 0;
            view.IsVisible = true;
            await view.FadeToAsync(1, duration);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Fade out animation
    /// </summary>
    public static async Task FadeOut(View view, uint duration = 250)
    {
        try
        {
            await view.FadeToAsync(0, duration);
            view.IsVisible = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Slide in from bottom
    /// </summary>
    public static async Task SlideInFromBottom(View view, uint duration = 300)
    {
        try
        {
            view.TranslationY = 50;
            view.Opacity = 0;
            view.IsVisible = true;
            
            await Task.WhenAll(
                view.TranslateToAsync(0, 0, duration),
                view.FadeToAsync(1, duration)
            );
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Pop in animation (scale + fade)
    /// </summary>
    public static async Task PopIn(View view, uint duration = 250)
    {
        try
        {
            view.Scale = 0.8;
            view.Opacity = 0;
            view.IsVisible = true;
            
            await Task.WhenAll(
                view.ScaleToAsync(1.0, duration, Easing.SpringOut),
                view.FadeToAsync(1, duration)
            );
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Reveal letter animation (used for hints)
    /// </summary>
    public static async Task RevealLetter(Label label)
    {
        try
        {
            await label.ScaleToAsync(1.3, 100);
            await label.ScaleToAsync(1.0, 100);

            // Flash color
            var originalColor = label.TextColor;
            label.TextColor = Colors.Gold;
            await Task.Delay(200);
            label.TextColor = originalColor;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Button tap feedback
    /// </summary>
    public static async Task ButtonTapFeedback(Button button)
    {
        try
        {
            await button.ScaleToAsync(0.95, 50);
            await button.ScaleToAsync(1.0, 50);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Animation error: {ex.Message}");
        }
    }

    /// <summary>
    /// Confetti celebration effect
    /// </summary>
    public static async Task Celebrate(View container)
    {
        try
        {
            // Create multiple confetti pieces
            var random = new Random();
            var confettiCount = 20;
            var confettiList = new List<BoxView>();

            for (int i = 0; i < confettiCount; i++)
            {
                var confetti = new BoxView
                {
                    WidthRequest = 10,
                    HeightRequest = 10,
                    BackgroundColor = GetRandomColor(random),
                    CornerRadius = 5,
                    Opacity = 0
                };

                if (container is Layout layout)
                {
                    layout.Add(confetti);
                    confettiList.Add(confetti);
                }
            }

            // Animate confetti
            var tasks = new List<Task>();
            foreach (var confetti in confettiList)
            {
                tasks.Add(AnimateConfettiPiece(confetti, random));
            }

            await Task.WhenAll(tasks);

            // Clean up
            if (container is Layout layout2)
            {
                foreach (var confetti in confettiList)
                {
                    layout2.Remove(confetti);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Confetti animation error: {ex.Message}");
        }
    }

    private static async Task AnimateConfettiPiece(BoxView confetti, Random random)
    {
        var startX = random.Next(-100, 100);
        var endX = startX + random.Next(-50, 50);
        var endY = random.Next(200, 400);

        confetti.TranslationX = startX;
        confetti.TranslationY = -50;
        
        await Task.WhenAll(
            confetti.FadeToAsync(1, 200),
            confetti.TranslateToAsync(endX, endY, 2000, Easing.CubicOut),
            confetti.RotateToAsync(random.Next(360, 720), 2000)
        );

        await confetti.FadeToAsync(0, 300);
    }

    private static Color GetRandomColor(Random random)
    {
        var colors = new[]
        {
            Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow,
            Colors.Orange, Colors.Purple, Colors.Pink, Colors.Cyan
        };
        return colors[random.Next(colors.Length)];
    }
}
