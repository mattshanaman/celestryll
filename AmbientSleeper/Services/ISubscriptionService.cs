using AmbientSleeper.Models;

namespace AmbientSleeper.Services
{
    public interface ISubscriptionService
    {
        SubscriptionTier CurrentTier { get; }
        bool IsLifetime { get; }
        event EventHandler<SubscriptionTier>? TierChanged;

        void SetTier(SubscriptionTier tier);
        void SetLifetimeTier(SubscriptionTier tier);
    }
}
