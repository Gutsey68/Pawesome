using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.Entities;

public class PasswordReset
{
    public int Id { get; set; }
    
    [MaxLength(255)]
    public required string Token { get; set; }
    
    public bool IsValid { get; set; } = true;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public required User User { get; set; }
}