// Models/Execution.cs
public class Execution
{
    public long ExecutionId { get; set; }
    public long BuyOrderId { get; set; }
    public long SellOrderId { get; set; }
    public string Symbol { get; set; } = "";
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;
}