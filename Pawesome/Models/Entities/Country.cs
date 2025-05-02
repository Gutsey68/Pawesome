using System.ComponentModel.DataAnnotations;

namespace Pawesome.Models.Entities;

public class Country
{
    public int Id { get; set; }
    
    [MaxLength(255)]
    public required string Name { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public required ICollection<City> Cities { get; set; }
}