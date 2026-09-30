// Models/Account.cs
public class Account
{
    public int AccountId { get; set; }
    public string OwnerName { get; set; } = "";
    public string? Email { get; set; } // stored lowercased; unique
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
