using Microsoft.EntityFrameworkCore;
using Pawesome.Data;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;

namespace Pawesome.Repositories;

/// <summary>
/// Repository for managing City entities in the database.
/// </summary>
public class CityRepository : Repository<City>, ICityRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CityRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public CityRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieves a city by its name and postal code.
    /// </summary>
    /// <param name="name">The name of the city.</param>
    /// <param name="postalCode">The postal code of the city.</param>
    /// <returns>The city entity if found; otherwise, null.</returns>
    public async Task<City?> GetByNameAndPostalCodeAsync(string name, string postalCode)
    {
        return await _context.Cities
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Name == name && c.PostalCode == postalCode);
    }
}
