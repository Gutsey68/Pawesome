namespace Pawesome.Models;

public class PetCartLandingViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Species { get; set; }
    public required string TagColor { get; set; }
    public required string Description { get; set; }
    
    public required string ImageLink { get; set; }
    
    public required string City { get; set; }
    public required string Country { get; set; }
}   