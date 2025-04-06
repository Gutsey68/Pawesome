using Pawesome.Models;

namespace Pawesome.Interfaces;

public interface IAnimalTypeRepository : IRepository<AnimalType>
{
    Task<IEnumerable<AnimalType>> GetAllAnimalTypesAsync();
    Task<AnimalType?> GetAnimalTypeByIdAsync(int id);
}