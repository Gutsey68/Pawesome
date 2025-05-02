using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.Entities;

public class Address
{
    public int Id { get; set; }
    
    [MaxLength(255)]
    public required string StreetAddress { get; set; }
    
    [MaxLength(255)]
    public string? AdditionalInfo { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int CityId { get; set; }
    
    public required City City { get; set; }
    public required ICollection<User> Users { get; set; }
}