using Plugin.InAppBilling;
using System.Security.Cryptography;
using System.Text;

namespace Pemdas.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly DatabaseService _databaseService;
        private bool _isSubscribed;
        private DateTime _lastStatusCheckUtc = DateTime.MinValue;
        private static readonly TimeSpan StatusCacheDuration = TimeSpan.FromMinutes(5);
        private const string EntitlementStateKey = "pemdas_sub_state";
        private const string EntitlementSignatureKey = "pemdas_sub_sig";

        // This is to be replaced with your actual Google Play Console product ID
        private const string ProductId = "pemdas_pro";

        public bool IsSubscribed => _isSubscribed;

        public SubscriptionService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<bool> CheckSubscriptionStatus()
        {
            try
            {
                var progress = await _databaseService.GetUserProgress();
                var trustedLocalEntitlement = await GetTrustedLocalEntitlementAsync(progress?.IsSubscribed ?? false);
                _isSubscribed = trustedLocalEntitlement;

                if (DateTime.UtcNow - _lastStatusCheckUtc < StatusCacheDuration)
                    return _isSubscribed;

                // Check actual stores if missing local flag or routinely validating
                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync();

                if (connected)
                {
                    try
                    {
                        var purchases = await billing.GetPurchasesAsync(ItemType.InAppPurchase);
                        var isPro = purchases?.Any(p => p.ProductId == ProductId) ?? false;

                        // Also check subscription just in case
                        if (!isPro)
                        {
                            var subs = await billing.GetPurchasesAsync(ItemType.Subscription);
                            isPro = isPro || (subs?.Any(p => p.ProductId == ProductId) ?? false);
                        }

                        if (progress != null && progress.IsSubscribed != isPro)
                        {
                            progress.IsSubscribed = isPro;
                            await _databaseService.UpdateUserProgress(progress);
                        }

                        _isSubscribed = isPro;
                        await SaveEntitlementAsync(isPro);
                        _lastStatusCheckUtc = DateTime.UtcNow;
                    }
                    finally
                    {
                        await billing.DisconnectAsync();
                    }
                }
                else
                {
                    // No store connectivity: only trust cryptographically signed local entitlement
                    if (progress != null && progress.IsSubscribed && !trustedLocalEntitlement)
                    {
                        progress.IsSubscribed = false;
                        await _databaseService.UpdateUserProgress(progress);
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback to local data
                Console.WriteLine(ex);
            }

            return _isSubscribed;
        }

        public async Task<bool> PurchaseSubscription()
        {
            try
            {
                var billing = CrossInAppBilling.Current;
                var connected = await billing.ConnectAsync();

                if (!connected)
                    return false;

                try
                {
                    var purchase = await billing.PurchaseAsync(ProductId, ItemType.Subscription);

                    if (purchase == null)
                    {
                        // Backward compatibility: allow one-time entitlement if store product is configured as IAP
                        purchase = await billing.PurchaseAsync(ProductId, ItemType.InAppPurchase);
                    }

                    if (purchase != null && purchase.State == PurchaseState.Purchased)
                    {
                        // Needs processing acknowledging depending on platforms
                        await billing.FinalizePurchaseAsync(new string[] { purchase.TransactionIdentifier });

                        var progress = await _databaseService.GetUserProgress();
                        if (progress != null)
                        {
                            progress.IsSubscribed = true;
                            await _databaseService.UpdateUserProgress(progress);
                            await SaveEntitlementAsync(true);
                            _isSubscribed = true;
                            _lastStatusCheckUtc = DateTime.UtcNow;
                            return true;
                        }
                    }
                }
                finally
                {
                    await billing.DisconnectAsync();
                }
            }
            catch (InAppBillingPurchaseException pEx)
            {
                // Handle different purchase exceptions
                Console.WriteLine(pEx);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }

            return false;
        }

        public async Task RestorePurchases()
        {
            _lastStatusCheckUtc = DateTime.MinValue;
            await CheckSubscriptionStatus();
        }

        private async Task<bool> GetTrustedLocalEntitlementAsync(bool dbSubscribedFlag)
        {
            if (!dbSubscribedFlag)
                return false;

            try
            {
                var state = await SecureStorage.GetAsync(EntitlementStateKey);
                var signature = await SecureStorage.GetAsync(EntitlementSignatureKey);

                if (state != "1" || string.IsNullOrWhiteSpace(signature))
                    return false;

                var expectedSignature = ComputeEntitlementSignature(state);
                return string.Equals(signature, expectedSignature, StringComparison.Ordinal);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private async Task SaveEntitlementAsync(bool isSubscribed)
        {
            try
            {
                var state = isSubscribed ? "1" : "0";
                var signature = ComputeEntitlementSignature(state);
                await SecureStorage.SetAsync(EntitlementStateKey, state);
                await SecureStorage.SetAsync(EntitlementSignatureKey, signature);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static string ComputeEntitlementSignature(string state)
        {
            var payload = $"{state}|{ProductId}|{AppInfo.PackageName}|{DeviceInfo.Manufacturer}|{DeviceInfo.Model}";
            var bytes = Encoding.UTF8.GetBytes(payload);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}