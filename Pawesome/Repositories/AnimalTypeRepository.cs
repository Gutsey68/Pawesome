using Microsoft.EntityFrameworkCore;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.Entities;

namespace Pawesome.Repositories;

/// <summary>
/// Repository for managing animal type entities in the database
/// </summary>
public class AnimalTypeRepository : Repository<AnimalType>, IAnimalTypeRepository
{
    /// <summary>
    /// Initializes a new instance of the AnimalTypeRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public AnimalTypeRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieves all animal types ordered by name
    /// </summary>
    /// <returns>A collection of animal types</returns>
    public async Task<IEnumerable<AnimalType>> GetAllAnimalTypesAsync()
    {
        return await _context.AnimalTypes.OrderBy(at => at.Name).ToListAsync();
    }

    /// <summary>
    /// Retrieves an animal type by its identifier
    /// </summary>
    /// <param name="id">The ID of the animal type to retrieve</param>
    /// <returns>The animal type if found, null otherwise</returns>
    public async Task<AnimalType?> GetAnimalTypeByIdAsync(int id)
    {
        return await _context.AnimalTypes.FindAsync(id);
    }
}