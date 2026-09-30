using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class StrategyController : ControllerBase
{
    private const int MaxCodeLength = 100_000;

    private readonly BrokerageContext _db;

    public StrategyController(BrokerageContext db)
    {
        _db = db;
    }

    public class CodeRequest
    {
        public string Code { get; set; } = "";
    }

    private int AccountId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /api/strategy
    [HttpGet]
    public async Task<IActionResult> GetStrategy()
    {
        var strategy = await _db.Strategies.FirstOrDefaultAsync(s => s.AccountId == AccountId);
        var lastSubmission = await _db.StrategySubmissions
            .Where(s => s.AccountId == AccountId)
            .OrderByDescending(s => s.StrategySubmissionId)
            .Select(s => new { s.StrategySubmissionId, s.Status, s.SubmittedAt })
            .FirstOrDefaultAsync();

        return Ok(new { code = strategy?.Code, updatedAt = strategy?.UpdatedAt, lastSubmission });
    }

    // PUT /api/strategy
    [HttpPut]
    public async Task<IActionResult> SaveStrategy([FromBody] CodeRequest request)
    {
        if (request.Code.Length > MaxCodeLength)
            return BadRequest($"Strategy code is limited to {MaxCodeLength:N0} characters.");

        var strategy = await Upsert(request.Code);
        await _db.SaveChangesAsync();

        return Ok(new { strategy.UpdatedAt });
    }

    // POST /api/strategy/submit
    // Saves the code and queues an immutable snapshot of it for the sandbox runner
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitStrategy([FromBody] CodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return BadRequest("Cannot submit an empty strategy.");
        if (request.Code.Length > MaxCodeLength)
            return BadRequest($"Strategy code is limited to {MaxCodeLength:N0} characters.");

        var strategy = await Upsert(request.Code);
        var submission = new StrategySubmission { AccountId = AccountId, Code = request.Code };
        _db.StrategySubmissions.Add(submission);
        await _db.SaveChangesAsync();

        _db.AuditLogs.Add(new AuditLog
        {
            AccountId = AccountId,
            Action = "STRATEGY_SUBMITTED",
            Detail = $"Submitted strategy #{submission.StrategySubmissionId}"
        });
        await _db.SaveChangesAsync();

        return Ok(new
        {
            strategy.UpdatedAt,
            submission.StrategySubmissionId,
            submission.Status,
            submission.SubmittedAt
        });
    }

    private async Task<Strategy> Upsert(string code)
    {
        var strategy = await _db.Strategies.FirstOrDefaultAsync(s => s.AccountId == AccountId);
        if (strategy == null)
        {
            strategy = new Strategy { AccountId = AccountId };
            _db.Strategies.Add(strategy);
        }

        strategy.Code = code;
        strategy.UpdatedAt = DateTime.UtcNow;
        return strategy;
    }
}
