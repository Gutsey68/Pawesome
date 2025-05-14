using Microsoft.EntityFrameworkCore;
using Pawesome.Data;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.Entities;

namespace Pawesome.Repositories;

/// <summary>
/// Repository for managing user entities in the database
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    /// <summary>
    /// Initializes a new instance of the UserRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public UserRepository(AppDbContext context) : base(context)
    {
    }
    
    /// <summary>
    /// Checks if a user with the specified email exists
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <returns>True if the email exists, false otherwise</returns>
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
    
    /// <summary>
    /// Retrieves a user by their email address
    /// </summary>
    /// <param name="email">The email of the user to retrieve</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    /// <summary>
    /// Retrieves a user by their ID including related address, city, country and pets
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve</param>
    /// <returns>The user with all related details if found, null otherwise</returns>
    public async Task<User?> GetUserByIdWithDetailsAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Address!.City)
            .ThenInclude(c => c.Country)
            .Include(u => u.Pets)
            .ThenInclude(p => p.AnimalType)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}