using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.Entities;

public class AnimalType
{
    public int Id { get; set; }
    
    [MaxLength(255)]
    public required string Name { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Pet>? Pets { get; set; }
    public ICollection<AnimalTypeAdvert> AnimalTypeAdverts { get; set; } = new List<AnimalTypeAdvert>();
}