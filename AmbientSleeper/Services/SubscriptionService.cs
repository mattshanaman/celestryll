using AmbientSleeper.Models;
using Microsoft.Maui.Storage;

namespace AmbientSleeper.Services;

public class SubscriptionService : ISubscriptionService
{
    private const string TierKey = "subscription_tier";
    private const string LifetimeKey = "subscription_lifetime";
    private SubscriptionTier _currentTier;

    public SubscriptionService()
    {
        var stored = Preferences.Default.Get(TierKey, nameof(SubscriptionTier.Free));
        if (!Enum.TryParse<SubscriptionTier>(stored, out _currentTier))
            _currentTier = SubscriptionTier.Free;

        IsLifetime = Preferences.Default.Get(LifetimeKey, false);
    }

    public SubscriptionTier CurrentTier => _currentTier;
    public bool IsLifetime { get; private set; }

    public event EventHandler<SubscriptionTier>? TierChanged;

    public void SetTier(SubscriptionTier tier)
    {
        if (_currentTier == tier && !IsLifetime) return;

        _currentTier = tier;
        Preferences.Default.Set(TierKey, tier.ToString());
        TierChanged?.Invoke(this, tier);
    }

    public void SetLifetimeTier(SubscriptionTier tier)
    {
        _currentTier = tier;
        IsLifetime = true;
        Preferences.Default.Set(TierKey, tier.ToString());
        Preferences.Default.Set(LifetimeKey, true);
        TierChanged?.Invoke(this, tier);
    }
}