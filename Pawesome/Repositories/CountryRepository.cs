using Microsoft.EntityFrameworkCore;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;

namespace Pawesome.Repositories;

/// <summary>
/// Repository for managing Country entities in the database.
/// </summary>
public class CountryRepository : Repository<Country>, ICountryRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CountryRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CountryRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieves a country by its name.
    /// </summary>
    /// <param name="name">The name of the country.</param>
    /// <returns>The country entity if found; otherwise, null.</returns>
    public async Task<Country?> GetByNameAsync(string name)
    {
        return await _context.Countries
            .FirstOrDefaultAsync(c => c.Name == name);
    }
}
