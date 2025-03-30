using Microsoft.AspNetCore.Identity;

namespace Pawesome.Models;

public class User : IdentityUser<int>
{
    public string LastName { get; set; }
    public string FirstName { get; set; }
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
    
    public int RoleId { get; set; }
    public int? AddressId { get; set; }
    
    public Role Role { get; set; }
    public Address Address { get; set; }
    public ICollection<Pet> Pets { get; set; }
    public ICollection<Notification> Notifications { get; set; }
    public ICollection<Report> Reports { get; set; }
    public ICollection<PasswordReset> PasswordResets { get; set; }
    public ICollection<Message> SentMessages { get; set; }
    public ICollection<Message> ReceivedMessages { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<Payment> Payments { get; set; }
}