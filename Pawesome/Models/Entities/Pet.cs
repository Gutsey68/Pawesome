using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.Entities;

public class Pet
{
    public int Id { get; set; }
    
    [MaxLength(255)]
    public required string Name { get; set; }
    
    [MaxLength(255)]
    public string? Breed { get; set; }
    
    public int? Age { get; set; }
    
    [MaxLength(255)]
    public string? Photo { get; set; }
    
    public string? Info { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public int AnimalTypeId { get; set; }

    public required User User { get; set; }
    public required AnimalType AnimalType { get; set; }
    public required ICollection<PetAdvert> PetAdverts { get; set; }
}