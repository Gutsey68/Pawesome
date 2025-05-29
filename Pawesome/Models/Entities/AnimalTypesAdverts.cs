namespace Pawesome.Models.Entities;

public class AnimalTypeAdvert
{
    public int AnimalTypeId { get; set; }
    public AnimalType AnimalType { get; set; } = null!;
    
    public int AdvertId { get; set; }
    public Advert Advert { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}