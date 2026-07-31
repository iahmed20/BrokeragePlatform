// Models/AuditLog.cs
public class AuditLog
{
    public long AuditLogId { get; set; }
    public int? AccountId { get; set; }   // nullable - some actions aren't account-specific
    public string Action { get; set; } = "";
    public string? Detail { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}