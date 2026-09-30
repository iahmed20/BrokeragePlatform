// Models/Strategy.cs
// The strategy code an account is currently editing (one per account)
public class Strategy
{
    public int StrategyId { get; set; }
    public int AccountId { get; set; }
    public string Code { get; set; } = "";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
