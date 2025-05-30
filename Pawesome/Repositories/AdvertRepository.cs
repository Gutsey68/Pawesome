using Microsoft.EntityFrameworkCore;
using Pawesome.Data;
using Pawesome.Interfaces;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Entities;
using Pawesome.Models.Enums;

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
    /// Retrieves all pet sitting adverts based on the type (offers or requests) and excludes cancelled ones
    /// </summary>
    /// <param name="isPetSitter">If true, returns pet sitting offers; if false, returns pet sitting requests</param>
    /// <returns>A list of adverts with their related entities</returns>
    public async Task<List<Advert>> GetAllAdvertsAsync(bool isPetSitter = false)
    {
        return await _context.Adverts
            .Include(a => a.User)
            .Include(a => a.PetAdverts)
            .ThenInclude(pa => pa.Pet)
            .ThenInclude(p => p!.AnimalType)
            .Where(a => a.Status != AdvertStatus.Cancelled && 
                        (isPetSitter ? a.Status == AdvertStatus.PendingOffer : a.Status == AdvertStatus.Pending))
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
            .ThenInclude(u => u.Address)
            .ThenInclude(a => a != null ? a.City : null)
            .Include(a => a.PetAdverts)
                .ThenInclude(pa => pa.Pet)
                    .ThenInclude(p => p!.User)
            .Include(a => a.PetAdverts)
                .ThenInclude(pa => pa.Pet)
                    .ThenInclude(p => p!.AnimalType)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
    
    /// <summary>
    /// Updates the status of an advert by its ID using a string status value.
    /// </summary>
    /// <param name="advertId">The ID of the advert to update.</param>
    /// <param name="status">The new status as a string (case-insensitive).</param>
    /// <returns>True if the update was successful; false if the advert was not found or the status is invalid.</returns>
    public async Task<bool> UpdateAdvertStatusAsync(int advertId, string status)
    {
        var advert = await _context.Adverts.FindAsync(advertId);

        if (advert == null)
            return false;

        if (Enum.TryParse<AdvertStatus>(status, true, out var advertStatus))
        {
            advert.Status = advertStatus;
            advert.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
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
        advert.Status = AdvertStatus.Pending;
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
        advert.Status = AdvertStatus.PendingOffer;
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
    public async Task<bool> UpdateAdvertStatusAsync(int advertId, AdvertStatus status)
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
    /// <param name="includeCancelled">Whether to include cancelled adverts</param>
    /// <returns>A list of adverts belonging to the specified user</returns>
    public async Task<List<Advert>> GetUserAdvertsAsync(int userId, bool includeCancelled = true)
    {
        var query = _context.Adverts
            .Include(a => a.User)
            .Include(a => a.PetAdverts)
            .ThenInclude(pa => pa.Pet)
            .ThenInclude(p => p!.AnimalType)
            .Where(a => a.UserId == userId);

        if (!includeCancelled)
        {
            query = query.Where(a => a.Status != AdvertStatus.Cancelled);
        }

        return await query.ToListAsync();
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
                .Include(a => a.Bookings)
                .FirstOrDefaultAsync(a => a.Id == advertId);

            if (advert == null)
                return false;

            // Suppression des associations
            _context.PetAdverts.RemoveRange(advert.PetAdverts);
            
            // Ne pas supprimer directement les bookings/payments car ils sont liés
            // La suppression doit être gérée par la cascade ou explicitement ailleurs
            
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
    /// <summary>
    /// Retrieves a filtered list of adverts based on the provided filter criteria.
    /// </summary>
    /// <param name="filter">The filter criteria to apply to the adverts query.</param>
    /// <returns>An enumerable of adverts matching the filter.</returns>
    public async Task<IEnumerable<Advert>> GetFilteredAdvertsAsync(AdvertFilterDto filter)
    {
        var query = _context.Adverts
            .Include(a => a.User)
                .ThenInclude(u => u.Address)
                    .ThenInclude(a => a != null ? a.City : null)
                        .ThenInclude(c => c != null ? c.Country : null)
            .Include(a => a.PetAdverts)
                .ThenInclude(pa => pa.Pet)
            .ThenInclude(p => p != null ? p.AnimalType : null)
            .Include(a => a.AnimalTypeAdverts)
                .ThenInclude(ata => ata.AnimalType)
            .AsQueryable();

        if (filter.IsPetSitterOffer.HasValue)
        {
                query = query.Where(a => (a.Status == AdvertStatus.PendingOffer) == filter.IsPetSitterOffer);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(a => a.Amount >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(a => a.Amount <= filter.MaxPrice.Value);
        }

        if (filter.StartDateFrom.HasValue)
        {
            query = query.Where(a => a.StartDate >= filter.StartDateFrom.Value);
        }

        if (filter.EndDateTo.HasValue)
        {
            query = query.Where(a => a.EndDate <= filter.EndDateTo.Value);
        }

        if (filter.CreatedAtFrom.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= filter.CreatedAtFrom.Value);
        }

        if (filter.CreatedAtTo.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= filter.CreatedAtTo.Value);
        }

        if (filter.AnimalTypeIds != null && filter.AnimalTypeIds.Count > 0)
        {
            query = query.Where(a => a.AnimalTypeAdverts.Any(ata => filter.AnimalTypeIds.Contains(ata.AnimalTypeId)));
        }

        if (filter.CountryId.HasValue)
        {
            query = query.Where(a => a.User.Address != null && 
                                     a.User.Address.City.CountryId == filter.CountryId.Value);
        }

        if (!string.IsNullOrEmpty(filter.City))
        {
            query = query.Where(a => a.User.Address != null && 
                                     a.User.Address.City.Name.ToLower().Contains(filter.City.ToLower()));
        }

        query = query.Where(a => a.Status != AdvertStatus.Cancelled && 
                                 (filter.IsPetSitterOffer.HasValue ? 
                                     (filter.IsPetSitterOffer.Value ? a.Status == AdvertStatus.PendingOffer : a.Status == AdvertStatus.Pending) : 
                                     (a.Status == AdvertStatus.Pending || a.Status == AdvertStatus.PendingOffer)));

        return await query.ToListAsync();
    }
}