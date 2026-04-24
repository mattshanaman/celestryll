using BadlyDefined.Models;
using BadlyDefined.Services;
using System.Diagnostics;

namespace BadlyDefined.Platforms;

/// <summary>
/// Mock subscription service for testing (replace with real implementation later)
/// </summary>
public class MockSubscriptionService : ISubscriptionService
{
    private bool _isSubscribed = false;

    public async Task<bool> CheckSubscriptionStatus()
    {
        await Task.Delay(100); // Simulate network call
        Debug.WriteLine($"💳 [MOCK] Subscription status: {_isSubscribed}");
        return _isSubscribed;
    }

    public async Task<bool> PurchaseSubscription(string productId)
    {
        await Task.Delay(500); // Simulate purchase flow
        _isSubscribed = true;
        Debug.WriteLine($"💳 [MOCK] Subscription purchased: {productId}");
        return true;
    }

    public async Task<bool> RestorePurchases()
    {
        await Task.Delay(300);
        Debug.WriteLine("💳 [MOCK] Purchases restored");
        return true;
    }

    public async Task<List<SubscriptionProduct>> GetAvailableProducts()
    {
        await Task.Delay(200);
        return new List<SubscriptionProduct>
        {
            new SubscriptionProduct
            {
                ProductId = "badlydefined.premium.monthly",
                Name = "BadlyDefined Premium",
                Description = "No ads, unlimited hints, all difficulties",
                Price = 2.99m,
                PriceString = "$2.99",
                Currency = "USD"
            }
        };
    }

    public async Task<bool> CancelSubscription()
    {
        await Task.Delay(200);
        _isSubscribed = false;
        Debug.WriteLine("💳 [MOCK] Subscription cancelled");
        return true;
    }
}
