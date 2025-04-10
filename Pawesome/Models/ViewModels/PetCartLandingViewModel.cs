namespace Pawesome.Models;

public class PetCartLandingViewModel
{
    public int Id { get; set; }
    public required string AnimalName { get; set; }
    public required string AnimalType { get; set; }
    public required string TagColor { get; set; }
    public required string Description { get; set; }
    
    public required string ImageUrl { get; set; }
    
    public required string City { get; set; }
    public required string Country { get; set; }
}   