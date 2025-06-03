using Microsoft.EntityFrameworkCore;
using Pawesome.Data;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;
using Pawesome.Models.Enums;

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
            .Include(u => u.Adverts.Where(a =>
                a.Status == AdvertStatus.Active || a.Status == AdvertStatus.Pending ||
                a.Status == AdvertStatus.PendingOffer))
            .ThenInclude(a => a.PetAdverts)
            .ThenInclude(pa => pa.Pet)
            .ThenInclude(p => p!.AnimalType)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    /// <summary>
    /// Updates the balance of a user by a specified amount.
    /// </summary>
    /// <param name="userId">The ID of the user whose balance will be updated.</param>
    /// <param name="amount">The amount to add (or subtract if negative) to the user's balance.</param>
    /// <returns>True if the update was successful, false if the user was not found.</returns>
    public async Task<bool> UpdateUserBalanceAsync(int userId, decimal amount)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.BalanceAccount += amount;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
    
    /// <summary>
    /// Marks the Stripe onboarding process as completed for a user.
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <returns>True if the update was successful, false if the user was not found.</returns>
    public async Task<bool> SetStripeOnboardingCompletedAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.IsStripeOnboardingCompleted = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
    
    /// <summary>
    /// Updates the user's balance to an exact specified amount.
    /// </summary>
    /// <param name="userId">The ID of the user whose balance will be updated.</param>
    /// <param name="amount">The exact amount to set as the user's balance.</param>
    /// <returns>True if the update was successful, false if the user was not found.</returns>
    public async Task<bool> UpdateUserBalanceToExactAmountAsync(int userId, decimal amount)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.BalanceAccount = amount;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }
}