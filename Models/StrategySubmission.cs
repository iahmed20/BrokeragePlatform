// Models/StrategySubmission.cs
// A snapshot of strategy code submitted for execution; later edits don't change it
public class StrategySubmission
{
    public int StrategySubmissionId { get; set; }
    public int AccountId { get; set; }
    public string Code { get; set; } = "";
    public string Status { get; set; } = "QUEUED";
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
}
