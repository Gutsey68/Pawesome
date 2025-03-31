namespace Pawesome.Models;

public class City
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string PostalCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int CountryId { get; set; }
    
    public required Country Country { get; set; }
    public required ICollection<Address> Addresses { get; set; }
}