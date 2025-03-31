namespace Pawesome.Models;

public class Pet
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Breed { get; set; }
    public int? Age { get; set; }
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