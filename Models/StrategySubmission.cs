// Models/StrategySubmission.cs
// A strategy version submitted for execution; versions are immutable, so later edits don't change it
public class StrategySubmission
{
    public int StrategySubmissionId { get; set; }
    public int AccountId { get; set; }
    public Guid StrategyId { get; set; }
    public int StrategyVersion { get; set; }
    public string Status { get; set; } = "QUEUED";
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public StrategyVersion? Version { get; set; }
}
