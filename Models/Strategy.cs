// Models/Strategy.cs
// A named strategy owned by an account; its code lives in StrategyVersions
public class Strategy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerId { get; set; } = ""; // AccountId as a string
    public string Name { get; set; } = "";
    public int CurrentVersion { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; } // soft delete

    public List<StrategyVersion> Versions { get; set; } = new();
}

// An immutable snapshot of a strategy's code; Version counts up from 1 per strategy
public class StrategyVersion
{
    public Guid StrategyId { get; set; }
    public int Version { get; set; }
    public string Code { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Note { get; set; } // optional "what I changed"

    public Strategy? Strategy { get; set; }
}
