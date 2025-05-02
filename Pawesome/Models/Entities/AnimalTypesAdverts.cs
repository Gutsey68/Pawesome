using Pawesome.Models.Entities;

namespace Pawesome.Models;

public class AnimalTypeAdvert
{
    public int AnimalTypeId { get; set; }
    public AnimalType AnimalType { get; set; } = null!;
    
    public int AdvertId { get; set; }
    public Advert Advert { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}