using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.AnimalType;

namespace Pawesome.Interfaces;

public interface IAnimalTypeService
{
    Task<List<AnimalTypeViewModel>> GetAllAnimalTypesAsync();
    Task<AnimalTypeViewModel?> GetAnimalTypeByIdAsync(int id);
}