namespace Pawesome.Models.DTOs;

public class PetSimpleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Photo { get; set; }
    public string AnimalTypeName { get; set; } = null!;
}