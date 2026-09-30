using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private static readonly TimeSpan LinkLifetime = TimeSpan.FromMinutes(15);

    private readonly BrokerageContext _db;
    private readonly IEmailSender _email;
    private readonly IConfiguration _config;

    public AuthController(BrokerageContext db, IEmailSender email, IConfiguration config)
    {
        _db = db;
        _email = email;
        _config = config;
    }

    public class RequestLinkRequest
    {
        public string Email { get; set; } = "";
        public string? OwnerName { get; set; } // only needed when creating an account
    }

    public class VerifyRequest
    {
        public string Token { get; set; } = "";
    }

    // POST /api/auth/request-link
    // Emails a sign-in link. The response is the same whether or not the account
    // exists, so this endpoint can't be used to discover who has an account.
    [HttpPost("request-link")]
    public async Task<IActionResult> RequestLink([FromBody] RequestLinkRequest request)
    {
        var email = NormalizeEmail(request.Email);
        if (email == null)
            return BadRequest("Please enter a valid email address.");

        var ownerName = request.OwnerName?.Trim();
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');

        _db.LoginTokens.Add(new LoginToken
        {
            TokenHash = Hash(token),
            Email = email,
            OwnerName = string.IsNullOrEmpty(ownerName) ? null : ownerName,
            ExpiresAt = DateTime.UtcNow.Add(LinkLifetime),
        });
        await _db.SaveChangesAsync();

        var frontendUrl = _config["Frontend:BaseUrl"]!.TrimEnd('/');
        var link = $"{frontendUrl}/?token={token}";
        await _email.SendAsync(email, "Your Brokerage sign-in link",
            $"<p>Click the link below to sign in. It expires in {LinkLifetime.TotalMinutes} minutes and can only be used once.</p>" +
            $"<p><a href=\"{link}\">Sign in to Brokerage</a></p>" +
            "<p>If you didn't request this, you can ignore this email.</p>");

        return Accepted(new { message = "If that address is valid, a sign-in link is on its way." });
    }

    // POST /api/auth/verify
    // Exchanges a magic-link token for a session cookie, creating the account on first sign-in.
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
            return BadRequest("Missing token.");

        var hash = Hash(request.Token);
        var now = DateTime.UtcNow;

        // Claim the token in a single UPDATE so the same link can't be redeemed twice concurrently
        var claimed = await _db.LoginTokens
            .Where(t => t.TokenHash == hash && t.UsedAt == null && t.ExpiresAt > now)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.UsedAt, now));

        if (claimed == 0)
            return Unauthorized("This sign-in link is invalid or has expired. Please request a new one.");

        var loginToken = await _db.LoginTokens.AsNoTracking().FirstAsync(t => t.TokenHash == hash);

        var account = await _db.Accounts.FirstOrDefaultAsync(a => a.Email == loginToken.Email);
        if (account == null)
        {
            account = new Account
            {
                Email = loginToken.Email,
                OwnerName = loginToken.OwnerName ?? loginToken.Email.Split('@')[0],
            };
            _db.Accounts.Add(account);
            await _db.SaveChangesAsync(); // assigns AccountId

            _db.AuditLogs.Add(new AuditLog
            {
                AccountId = account.AccountId,
                Action = "ACCOUNT_CREATED",
                Detail = $"Account created for {account.Email} via email link"
            });
        }

        _db.AuditLogs.Add(new AuditLog
        {
            AccountId = account.AccountId,
            Action = "LOGIN",
            Detail = $"Signed in via email link"
        });
        await _db.SaveChangesAsync();

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
            new Claim(ClaimTypes.Name, account.OwnerName),
            new Claim(ClaimTypes.Email, account.Email!),
        }, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });

        return Ok(new { account.AccountId, account.OwnerName, account.Email });
    }

    // GET /api/auth/me
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var account = await _db.Accounts.FindAsync(accountId);
        if (account == null) return Unauthorized();

        return Ok(new { account.AccountId, account.OwnerName, account.Email });
    }

    // POST /api/auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }

    private static string? NormalizeEmail(string? email)
    {
        email = email?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(email) || !MailAddress.TryCreate(email, out var parsed) || parsed.Address != email)
            return null;
        return email;
    }

    private static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
