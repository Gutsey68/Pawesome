namespace Pawesome.Models.Dtos.Pet;

public class CreatePetDto
{
    public required string Name { get; set; }
    public string? Breed { get; set; }
    public int? Age { get; set; }
    public string? Info { get; set; }
    public int AnimalTypeId { get; set; }
    public IFormFile? Photo { get; set; }
}