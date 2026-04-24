namespace Pemdas.Services
{
    public interface ISubscriptionService
    {
        Task<bool> CheckSubscriptionStatus();
        Task<bool> PurchaseSubscription();
        Task RestorePurchases();
        bool IsSubscribed { get; }
    }
}