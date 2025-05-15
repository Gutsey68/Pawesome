namespace Pawesome.Models.ViewModels.Pet;

public class PetViewModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Species { get; set; }
    public required string Photo { get; set; }
}