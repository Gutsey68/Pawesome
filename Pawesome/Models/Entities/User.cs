using Microsoft.AspNetCore.Identity;

namespace Pawesome.Models;

public class User : IdentityUser<int>
{
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? Bio { get; set; }
    public string? Photo { get; set; }
    public float? Rating { get; set; }
    public string? Status { get; set; }
    public bool IsVerified { get; set; } = false;
    public decimal BalanceAccount { get; set; } = 0;
    public int OnboardingStep { get; set; } = 1;
    public bool IsOnboardingCompleted { get; set; } = false;
    public int CompletedProfile { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int? AddressId { get; set; }
    
    public required Address Address { get; set; }
    public required ICollection<Pet> Pets { get; set; }
    public required ICollection<Notification> Notifications { get; set; }
    public required ICollection<Report> Reports { get; set; }
    public required ICollection<PasswordReset> PasswordResets { get; set; }
    public required ICollection<Message> SentMessages { get; set; }
    public required ICollection<Message> ReceivedMessages { get; set; }
    public required ICollection<Review> Reviews { get; set; }
    public required ICollection<Payment> Payments { get; set; }
}