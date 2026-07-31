// Models/Account.cs
public class Account
{
    public int AccountId { get; set; }
    public string OwnerName { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}