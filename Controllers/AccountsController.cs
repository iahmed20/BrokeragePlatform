using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly BrokerageContext _db;

    public AccountsController(BrokerageContext db)
    {
        _db = db;
    }

    // POST /api/accounts
    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] string ownerName)
    {
        var account = new Account { OwnerName = ownerName };
        _db.Accounts.Add(account);
        await _db.SaveChangesAsync();
        return Ok(account);
    }

    // GET /api/accounts/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAccount(int id)
    {
        var account = await _db.Accounts.FindAsync(id);
        if (account == null) return NotFound();

        var cashBalance = await _db.LedgerEntries
            .Where(l => l.AccountId == id && l.EntryType == "CASH")
            .SumAsync(l => (decimal?)l.Amount) ?? 0;

        return Ok(new { account.AccountId, account.OwnerName, CashBalance = cashBalance });
    }

    public class DepositRequest
    {
        public decimal Amount { get; set; }
    }

    // POST /api/accounts/5/deposit
    [HttpPost("{id}/deposit")]
    public async Task<IActionResult> Deposit(int id, [FromBody] DepositRequest request)
    {
        if (request.Amount <= 0)
            return BadRequest("Deposit amount must be positive, nyaa.");

        var account = await _db.Accounts.FindAsync(id);
        if (account == null) return NotFound();

        var entry = new LedgerEntry
        {
            AccountId = id,
            EntryType = "CASH",
            Symbol = null,
            Amount = request.Amount,
            ReferenceType = "DEPOSIT",
            ReferenceId = 0
        };

        _db.LedgerEntries.Add(entry);

        _db.AuditLogs.Add(new AuditLog
        {
            AccountId = id,
            Action = "DEPOSIT",
            Detail = $"Deposited {request.Amount:C} into account {id}"
        });

        await _db.SaveChangesAsync();

        var newBalance = await _db.LedgerEntries
            .Where(l => l.AccountId == id && l.EntryType == "CASH")
            .SumAsync(l => (decimal?)l.Amount) ?? 0;

        return Ok(new { AccountId = id, DepositedAmount = request.Amount, NewBalance = newBalance });
    }
}