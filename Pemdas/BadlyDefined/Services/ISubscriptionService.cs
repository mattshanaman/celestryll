namespace BadlyDefined.Services;

/// <summary>
/// Interface for subscription/premium services
/// </summary>
public interface ISubscriptionService
{
    /// <summary>
    /// Check current subscription status
    /// </summary>
    Task<bool> CheckSubscriptionStatus();

    /// <summary>
    /// Purchase a subscription
    /// </summary>
    /// <param name="productId">Product ID to purchase</param>
    Task<bool> PurchaseSubscription(string productId);

    /// <summary>
    /// Restore previous purchases
    /// </summary>
    Task<bool> RestorePurchases();

    /// <summary>
    /// Get available subscription products
    /// </summary>
    Task<List<SubscriptionProduct>> GetAvailableProducts();

    /// <summary>
    /// Cancel active subscription
    /// </summary>
    Task<bool> CancelSubscription();
}
