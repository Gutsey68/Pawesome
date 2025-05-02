using Microsoft.EntityFrameworkCore;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.Entities;

namespace Pawesome.Repositories;

/// <summary>
/// Repository for managing pet entities in the database
/// </summary>
public class PetRepository : Repository<Pet>, IPetRepository
{
    /// <summary>
    /// Initializes a new instance of the PetRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public PetRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Retrieves all pets for a specific user including their animal type
    /// </summary>
    /// <param name="userId">The ID of the user whose pets to retrieve</param>
    /// <returns>A collection of pets belonging to the specified user</returns>
    public async Task<IEnumerable<Pet>> GetPetsByUserIdAsync(int userId)
    {
        return await _context.Pets
            .Include(p => p.AnimalType)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a pet by its ID including related animal type and user details
    /// </summary>
    /// <param name="id">The ID of the pet to retrieve</param>
    /// <returns>The pet with its details if found, null otherwise</returns>
    public async Task<Pet?> GetPetWithDetailsAsync(int id)
    {
        return await _context.Pets
            .Include(p => p.AnimalType)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}