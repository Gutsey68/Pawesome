using Pawesome.Models.Dtos.AnimalType;

namespace Pawesome.Models.ViewModels.AnimalType;

public class AnimalTypeViewModel
{
    public required AnimalTypeDto AnimalType { get; set; }
    public bool IsSelected { get; set; }
    public int Id => AnimalType.Id;
    public string Name => AnimalType?.Name ?? string.Empty;
}
