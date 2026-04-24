namespace AmbientSleeper.Services;

/// <summary>
/// Manages ad-based rewards for Free tier users
/// </summary>
public class AdRewardManager : IAdRewardManager
{
    private DateTime? _sessionExtensionExpiry;
    private readonly HashSet<string> _unlockedBundles = new();
    private readonly TimeSpan _defaultExtensionDuration = TimeSpan.FromMinutes(45); // Match Free tier limit
    
    public bool HasSessionExtension => _sessionExtensionExpiry.HasValue && _sessionExtensionExpiry.Value > DateTime.UtcNow;
    
    public TimeSpan RemainingExtensionTime
    {
        get
        {
            if (!HasSessionExtension)
                return TimeSpan.Zero;
            
            var remaining = _sessionExtensionExpiry!.Value - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }
    
    public void GrantSessionExtension(TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero)
            duration = _defaultExtensionDuration;
        
        // If already have extension, add to it; otherwise start new
        if (HasSessionExtension)
        {
            _sessionExtensionExpiry = _sessionExtensionExpiry!.Value.Add(duration);
        }
        else
        {
            _sessionExtensionExpiry = DateTime.UtcNow.Add(duration);
        }
        
        System.Diagnostics.Debug.WriteLine($"[AdReward] Session extended by {duration.TotalMinutes} minutes. Expires: {_sessionExtensionExpiry}");
    }
    
    public bool IsSoundUnlocked(string bundleId)
    {
        return _unlockedBundles.Contains(bundleId);
    }
    
    public void UnlockSound(string bundleId)
    {
        if (string.IsNullOrWhiteSpace(bundleId))
            return;
        
        _unlockedBundles.Add(bundleId);
        System.Diagnostics.Debug.WriteLine($"[AdReward] Unlocked bundle: {bundleId}");
    }
    
    public void ClearSessionUnlocks()
    {
        _unlockedBundles.Clear();
        _sessionExtensionExpiry = null;
        System.Diagnostics.Debug.WriteLine("[AdReward] Cleared all session unlocks");
    }
    
    public IReadOnlyList<string> GetUnlockedBundles()
    {
        return _unlockedBundles.ToList();
    }
}
