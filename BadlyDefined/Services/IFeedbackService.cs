namespace BadlyDefined.Services;

/// <summary>
/// Interface for haptic feedback and sound effects
/// </summary>
public interface IFeedbackService
{
    /// <summary>
    /// Play success feedback (haptic + sound)
    /// </summary>
    Task PlaySuccessFeedback();

    /// <summary>
    /// Play error feedback (haptic + sound)
    /// </summary>
    Task PlayErrorFeedback();

    /// <summary>
    /// Play light tap feedback
    /// </summary>
    Task LightTap();

    /// <summary>
    /// Play medium impact feedback
    /// </summary>
    Task MediumImpact();

    /// <summary>
    /// Play heavy impact feedback
    /// </summary>
    Task HeavyImpact();
}
