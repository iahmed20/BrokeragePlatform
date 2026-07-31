// Models/LedgerEntry.cs
public class LedgerEntry
{
    public long LedgerEntryId { get; set; }
    public int AccountId { get; set; }
    public string EntryType { get; set; } = ""; // "CASH" or "POSITION"
    public string? Symbol { get; set; }
    public decimal Amount { get; set; }
    public string ReferenceType { get; set; } = "";
    public long ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}