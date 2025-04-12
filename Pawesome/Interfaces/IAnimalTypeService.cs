using Pawesome.Models.ViewModels;

namespace Pawesome.Interfaces;

public interface IAnimalTypeService
{
    Task<List<AnimalTypeViewModel>> GetAllAnimalTypesAsync();
    Task<AnimalTypeViewModel?> GetAnimalTypeByIdAsync(int id);
}