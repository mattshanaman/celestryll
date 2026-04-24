namespace BadlyDefined.Services;

/// <summary>
/// Represents a subscription product available for purchase
/// </summary>
public class SubscriptionProduct
{
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string PriceString { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
}
