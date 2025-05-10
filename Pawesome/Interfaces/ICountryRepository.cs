using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface ICountryRepository : IRepository<Country>
{
    Task<Country?> GetByNameAsync(string name);
}