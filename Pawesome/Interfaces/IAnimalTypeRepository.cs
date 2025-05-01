using Pawesome.Models;
using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface IAnimalTypeRepository : IRepository<AnimalType>
{
    Task<IEnumerable<AnimalType>> GetAllAnimalTypesAsync();
    Task<AnimalType?> GetAnimalTypeByIdAsync(int id);
}