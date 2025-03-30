namespace Pawesome.Models;

public class PasswordReset
{
    public int Id { get; set; }
    public string Token { get; set; }
    public bool IsValid { get; set; } = true;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User User { get; set; }
}