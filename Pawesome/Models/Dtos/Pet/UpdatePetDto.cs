namespace Pawesome.Models.Dtos.Pet;

public class UpdatePetDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Breed { get; set; }
    public int? Age { get; set; }
    public string? Info { get; set; }
    public int AnimalTypeId { get; set; }
    public IFormFile? Photo { get; set; }
    public string? ExistingPhoto { get; set; }
}