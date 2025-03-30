namespace Pawesome.Models;

public class Address
{
    public int Id { get; set; }
    public string StreetAddress { get; set; }
    public string? AdditionalInfo { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int CityId { get; set; }
    
    public City City { get; set; }
    public ICollection<User> Users { get; set; }
}