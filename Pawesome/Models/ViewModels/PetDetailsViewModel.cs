namespace Pawesome.Models.ViewModels;

public class PetDetailsViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Breed { get; set; }
    public int? Age { get; set; }
    public string? Photo { get; set; }
    public string? Info { get; set; }
    public required string Species { get; set; }
    public int UserId { get; set; }
    public required string OwnerName { get; set; }
    public DateTime CreatedAt { get; set; }
}