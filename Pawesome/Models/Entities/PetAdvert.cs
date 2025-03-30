namespace Pawesome.Models;

public class PetAdvert
{
    public int PetId { get; set; }
    public int AdvertId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public Pet Pet { get; set; }
    public Advert Advert { get; set; }
}