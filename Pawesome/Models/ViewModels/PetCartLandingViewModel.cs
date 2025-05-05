namespace Pawesome.Models.ViewModels;

public class PetCartViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Species { get; set; }
    public required string Info { get; set; }
    
    public required string Photo { get; set; }
    
    public string? UserId { get; set; }
    
    public string? AddressId { get; set; }
    
    public required string City { get; set; }
    public required string Country { get; set; }
}   