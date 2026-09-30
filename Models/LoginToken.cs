// Models/LoginToken.cs
// A single-use magic link. Only the SHA-256 hash of the token is stored,
// so a leaked database can't be used to sign in.
public class LoginToken
{
    public int LoginTokenId { get; set; }
    public string TokenHash { get; set; } = "";
    public string Email { get; set; } = "";
    public string? OwnerName { get; set; } // used if this link creates a new account
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
}
