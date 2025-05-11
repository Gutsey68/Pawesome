using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Pawesome.Models.Entities;

public class User : IdentityUser<int>
{

    [MaxLength(255)]
    public required string LastName { get; set; }
    
    [MaxLength(255)]
    public required string FirstName { get; set; }
    
    public string? Bio { get; set; }
    
    [MaxLength(255)]
    public string? Photo { get; set; }
    
    public float? Rating { get; set; }
    
    [MaxLength(255)]
    public string? Status { get; set; }
    
    public bool IsVerified { get; set; } = false;
    
    public decimal BalanceAccount { get; set; } = 0;
    
    public int OnboardingStep { get; set; } = 1;
    
    public bool IsOnboardingCompleted { get; set; } = false;
    
    [MaxLength(100)]
    public int CompletedProfile { get; set; } = 0;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int? AddressId { get; set; }
    
    public Address? Address { get; set; }
    public required ICollection<Pet> Pets { get; set; }
    public required ICollection<Notification> Notifications { get; set; }
    public required ICollection<Report> Reports { get; set; }
    public required ICollection<PasswordReset> PasswordResets { get; set; }
    public required ICollection<Message> SentMessages { get; set; }
    public required ICollection<Message> ReceivedMessages { get; set; }
    public required ICollection<Review> Reviews { get; set; }
    public required ICollection<Payment> Payments { get; set; }
    public ICollection<Advert> Adverts { get; set; } = new List<Advert>();
}