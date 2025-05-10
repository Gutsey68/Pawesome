using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface ICityRepository : IRepository<City>
{
    Task<City?> GetByNameAndPostalCodeAsync(string name, string postalCode);
}