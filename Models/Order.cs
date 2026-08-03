// Models/Order.cs
public class Order
{
    public long OrderId { get; set; }
    public int AccountId { get; set; }
    public string Symbol { get; set; } = "";
    public string Side { get; set; } = "";       // "BUY" or "SELL"
    public string OrderType { get; set; } = "";  // "MARKET" or "LIMIT"
    public decimal? LimitPrice { get; set; }      // null for market orders
    public decimal Quantity { get; set; }
    public decimal QuantityFilled { get; set; } = 0;
    public string Status { get; set; } = "OPEN";  // OPEN, PARTIAL, FILLED, CANCELLED, REJECTED
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal RemainingQty => Quantity - QuantityFilled;
    
}