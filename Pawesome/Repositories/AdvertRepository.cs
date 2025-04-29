using Microsoft.EntityFrameworkCore;
using Pawesome.Interfaces;
using Pawesome.Models;

namespace Pawesome.Repositories;

/// <summary>
/// Repository handling database operations for pet sitting adverts
/// </summary>
public class AdvertRepository : IAdvertRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the AdvertRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public AdvertRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all pet sitting adverts based on the type (offers or requests)
    /// </summary>
    /// <param name="isPetSitter">If true, returns pet sitting offers; if false, returns pet sitting requests</param>
    /// <returns>A list of adverts with their related entities</returns>
    public async Task<List<Advert>> GetAllAdvertsAsync(bool isPetSitter = false)
    {
        return await _context.Adverts
            .Include(a => a.User)
            .Include(a => a.User)
            .Include(a => a.PetAdverts)
            .ThenInclude(pa => pa.Pet)
            .ThenInclude(p => p!.AnimalType)
            .Where(a => isPetSitter ? a.Status == "pending_offer" : a.Status == "pending")
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific advert by its ID with related entities
    /// </summary>
    /// <param name="id">The ID of the advert to retrieve</param>
    /// <returns>The advert if found, null otherwise</returns>
    public async Task<Advert?> GetAdvertByIdAsync(int id)
    {
        return await _context.Adverts
            .Include(a => a.User) 
            .Include(a => a.PetAdverts)
                .ThenInclude(pa => pa.Pet)
                    .ThenInclude(p => p!.User)
            .Include(a => a.PetAdverts)
                .ThenInclude(pa => pa.Pet)
                    .ThenInclude(p => p!.AnimalType)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <summary>
    /// Creates a new pet sitting request and associates it with the specified pets
    /// </summary>
    /// <param name="advert">The advert to create</param>
    /// <param name="petIds">List of pet IDs to associate with this request</param>
    /// <param name="userId">ID of the user creating the request</param>
    /// <returns>The created advert with its ID populated</returns>
    public async Task<Advert> CreatePetSittingRequestAsync(Advert advert, List<int> petIds, int userId)
    {
        advert.Status = "pending";
        advert.StartDate = DateTime.SpecifyKind(advert.StartDate, DateTimeKind.Utc);
        advert.EndDate = DateTime.SpecifyKind(advert.EndDate, DateTimeKind.Utc);
        advert.CreatedAt = DateTime.UtcNow;
        advert.UpdatedAt = DateTime.UtcNow;
        advert.UserId = userId;

        await _context.Adverts.AddAsync(advert);
        await _context.SaveChangesAsync();

        foreach (var petId in petIds)
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet != null)
            {
                await _context.PetAdverts.AddAsync(new PetAdvert
                {
                    PetId = petId,
                    AdvertId = advert.Id,
                    Pet = pet,
                    Advert = advert
                });
            }
        }

        await _context.SaveChangesAsync();
        return advert;
    }

    /// <summary>
    /// Creates a new pet sitting offer and associates it with the specified animal types
    /// </summary>
    /// <param name="advert">The advert to create</param>
    /// <param name="animalTypeIds">List of animal type IDs that the pet sitter accepts</param>
    /// <param name="userId">ID of the user creating the offer</param>
    /// <returns>The created advert with its ID populated</returns>
    public async Task<Advert> CreatePetSittingOfferAsync(Advert advert, List<int> animalTypeIds, int userId)
    {
        advert.Status = "pending_offer";
        advert.StartDate = DateTime.SpecifyKind(advert.StartDate, DateTimeKind.Utc);
        advert.EndDate = DateTime.SpecifyKind(advert.EndDate, DateTimeKind.Utc);
        advert.CreatedAt = DateTime.UtcNow;
        advert.UpdatedAt = DateTime.UtcNow;
        advert.UserId = userId;

        await _context.Adverts.AddAsync(advert);
        await _context.SaveChangesAsync();

        foreach (var animalTypeId in animalTypeIds)
        {
            var petsOfType = await _context.Pets
                .Where(p => p.AnimalTypeId == animalTypeId)
                .ToListAsync();

            var firstPet = petsOfType.FirstOrDefault();
            if (firstPet != null)
            {
                await _context.PetAdverts.AddAsync(new PetAdvert
                {
                    PetId = firstPet.Id,
                    AdvertId = advert.Id,
                    Pet = firstPet,
                    Advert = advert
                });
            }
        }

        await _context.SaveChangesAsync();
        return advert;
    }

    /// <summary>
    /// Updates the status of an existing advert
    /// </summary>
    /// <param name="advertId">The ID of the advert to update</param>
    /// <param name="status">The new status value</param>
    /// <returns>True if the update was successful, false if the advert wasn't found</returns>
    public async Task<bool> UpdateAdvertStatusAsync(int advertId, string status)
    {
        var advert = await _context.Adverts.FindAsync(advertId);
        if (advert == null)
            return false;

        advert.Status = status;
        advert.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Retrieves all adverts associated with a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose adverts to retrieve</param>
    /// <returns>A list of adverts belonging to the specified user</returns>
    public async Task<List<Advert>> GetUserAdvertsAsync(int userId)
    {
        return await _context.Adverts
            .Include(a => a.User)
            .Include(a => a.PetAdverts)
            .ThenInclude(pa => pa.Pet)
            .ThenInclude(p => p!.AnimalType)
            .Where(a => a.UserId == userId)
            .ToListAsync();
    }

    /// <summary>
    /// Updates an existing pet sitting request and its associated pets
    /// </summary>
    /// <param name="advert">The updated advert data</param>
    /// <param name="petIds">List of pet IDs to associate with the advert</param>
    /// <returns>The updated advert with its related entities</returns>
    public async Task<Advert> UpdatePetSittingRequestAsync(Advert advert, List<int> petIds)
    {
        var existingAdvert = await _context.Adverts
            .Include(a => a.PetAdverts)
            .FirstOrDefaultAsync(a => a.Id == advert.Id);
            
        if (existingAdvert == null)
            throw new InvalidOperationException($"Advert with ID {advert.Id} not found");
        
        existingAdvert.StartDate = DateTime.SpecifyKind(advert.StartDate, DateTimeKind.Utc);
        existingAdvert.EndDate = DateTime.SpecifyKind(advert.EndDate, DateTimeKind.Utc);
        existingAdvert.Amount = advert.Amount;
        existingAdvert.AdditionalInformation = advert.AdditionalInformation;
        existingAdvert.UpdatedAt = DateTime.UtcNow;
        
        _context.PetAdverts.RemoveRange(existingAdvert.PetAdverts);
        
        foreach (var petId in petIds)
        {
            var pet = await _context.Pets.FindAsync(petId);
            if (pet != null)
            {
                await _context.PetAdverts.AddAsync(new PetAdvert
                {
                    PetId = petId,
                    AdvertId = existingAdvert.Id,
                    Pet = pet,
                    Advert = existingAdvert
                });
            }
        }
        
        await _context.SaveChangesAsync();
        
        return await GetAdvertByIdAsync(existingAdvert.Id) ?? existingAdvert;
    }

    /// <summary>
    /// Updates an existing pet sitting offer and its associated animal types
    /// </summary>
    /// <param name="advert">The updated advert data</param>
    /// <param name="animalTypeIds">List of animal type IDs that the pet sitter accepts</param>
    /// <returns>The updated advert with its related entities</returns>
    public async Task<Advert> UpdatePetSittingOfferAsync(Advert advert, List<int> animalTypeIds)
    {
        var existingAdvert = await _context.Adverts
            .Include(a => a.PetAdverts)
            .FirstOrDefaultAsync(a => a.Id == advert.Id);
            
        if (existingAdvert == null)
            throw new InvalidOperationException($"Advert with ID {advert.Id} not found");
        
        existingAdvert.StartDate = DateTime.SpecifyKind(advert.StartDate, DateTimeKind.Utc);
        existingAdvert.EndDate = DateTime.SpecifyKind(advert.EndDate, DateTimeKind.Utc);
        existingAdvert.Amount = advert.Amount;
        existingAdvert.AdditionalInformation = advert.AdditionalInformation;
        existingAdvert.UpdatedAt = DateTime.UtcNow;
        
        _context.PetAdverts.RemoveRange(existingAdvert.PetAdverts);
        
        foreach (var animalTypeId in animalTypeIds)
        {
            var petsOfType = await _context.Pets
                .Where(p => p.AnimalTypeId == animalTypeId)
                .ToListAsync();

            var firstPet = petsOfType.FirstOrDefault();
            if (firstPet != null)
            {
                await _context.PetAdverts.AddAsync(new PetAdvert
                {
                    PetId = firstPet.Id,
                    AdvertId = existingAdvert.Id,
                    Pet = firstPet,
                    Advert = existingAdvert
                });
            }
        }
        
        await _context.SaveChangesAsync();
        
        return await GetAdvertByIdAsync(existingAdvert.Id) ?? existingAdvert;
    }
    
    /// <summary>
    /// Deletes an advert and all its associated entities
    /// </summary>
    /// <param name="advertId">The ID of the advert to delete</param>
    /// <returns>True if the advert was deleted successfully, false if it wasn't found</returns>
    public async Task<bool> DeleteAdvertAsync(int advertId)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var advert = await _context.Adverts
                .Include(a => a.PetAdverts)
                .Include(a => a.Reviews)
                .Include(a => a.Payments)
                .FirstOrDefaultAsync(a => a.Id == advertId);

            if (advert == null)
                return false;

            _context.PetAdverts.RemoveRange(advert.PetAdverts);
            _context.Reviews.RemoveRange(advert.Reviews);
            _context.Payments.RemoveRange(advert.Payments);
        
            _context.Adverts.Remove(advert);
            await _context.SaveChangesAsync();
        
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}