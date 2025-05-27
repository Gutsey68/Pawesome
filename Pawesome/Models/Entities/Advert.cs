using System.ComponentModel.DataAnnotations;
using Pawesome.Models.Enums;

namespace Pawesome.Models.Entities;

public class Advert
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public AdvertStatus Status { get; set; } = AdvertStatus.Pending;
    public decimal Amount { get; set; }
    [MaxLength(255)]
    public string? AdditionalInformation { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public required ICollection<PetAdvert> PetAdverts { get; set; }
    public required ICollection<AnimalTypeAdvert> AnimalTypeAdverts { get; set; }
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}