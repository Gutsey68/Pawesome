using AutoMapper;
using Azure.Core;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.Advert;

namespace Pawesome.Services;

/// <summary>
/// Service handling pet sitting advertisements operations
/// </summary>
public class AdvertService : IAdvertService
{
    private readonly IAdvertRepository _repository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the PetSittingService
    /// </summary>
    /// <param name="repository">Repository for pet sitting data operations</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    public AdvertService(IAdvertRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all pet sitting adverts based on the type
    /// </summary>
    /// <param name="isPetSitter">If true, returns pet sitting offers; if false, returns pet sitting requests</param>
    /// <returns>A list of pet sitting advertisement DTOs</returns>
    public async Task<List<PetSittingAdvertDto>> GetAllAdvertsAsync(bool isPetSitter = false)
    {
        var adverts = await _repository.GetAllAdvertsAsync(isPetSitter);
        return _mapper.Map<List<PetSittingAdvertDto>>(adverts);
    }

    /// <summary>
    /// Retrieves a specific pet sitting advert by its ID
    /// </summary>
    /// <param name="id">The ID of the advert to retrieve</param>
    /// <returns>The pet sitting advertisement DTO if found, null otherwise</returns>
    public async Task<PetSittingAdvertDto?> GetAdvertByIdAsync(int id)
    {
        var advert = await _repository.GetAdvertByIdAsync(id);
        return advert != null ? _mapper.Map<PetSittingAdvertDto>(advert) : null;
    }

    /// <summary>
    /// Creates a new pet sitting request
    /// </summary>
    /// <param name="model">Data for the pet sitting request</param>
    /// <param name="userId">ID of the user creating the request</param>
    /// <returns>The created pet sitting advertisement DTO</returns>
    public async Task<PetSittingAdvertDto> CreatePetSittingRequestAsync(PetSittingRequestViewModel model, int userId)
    {
        var advert = new Advert
        {
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Amount = model.Amount,
            AdditionalInformation = model.AdditionalInformation,
            Status = "pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PetAdverts = new List<PetAdvert>(),
            Reviews = new List<Review>(),
            Payments = new List<Payment>()
        };

        var createdAdvert = await _repository.CreatePetSittingRequestAsync(advert, model.PetIds, userId);
        return _mapper.Map<PetSittingAdvertDto>(createdAdvert);
    }

    /// <summary>
    /// Creates a new pet sitting offer
    /// </summary>
    /// <param name="model">Data for the pet sitting offer</param>
    /// <param name="userId">ID of the user creating the offer</param>
    /// <returns>The created pet sitting advertisement DTO</returns>
    public async Task<PetSittingAdvertDto> CreatePetSittingOfferAsync(PetSittingOfferViewModel model, int userId)
    {
        var advert = new Advert
        {
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Amount = model.Amount,
            AdditionalInformation = model.AdditionalInformation,
            Status = "pending_offer",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PetAdverts = new List<PetAdvert>(),
            Reviews = new List<Review>(),
            Payments = new List<Payment>()
        };

        var createdAdvert = await _repository.CreatePetSittingOfferAsync(advert, model.AcceptedAnimalTypeIds, userId);
        return _mapper.Map<PetSittingAdvertDto>(createdAdvert);
    }

    /// <summary>
    /// Updates the status of an existing advert
    /// </summary>
    /// <param name="advertId">The ID of the advert to update</param>
    /// <param name="status">The new status value</param>
    /// <returns>True if the update was successful, false if the advert wasn't found</returns>
    public async Task<bool> UpdateAdvertStatusAsync(int advertId, string status)
    {
        return await _repository.UpdateAdvertStatusAsync(advertId, status);
    }

    /// <summary>
    /// Retrieves all adverts associated with a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose adverts to retrieve</param>
    /// <returns>A list of pet sitting advertisement DTOs belonging to the specified user</returns>
    public async Task<List<PetSittingAdvertDto>> GetUserAdvertsAsync(int userId)
    {
        var adverts = await _repository.GetUserAdvertsAsync(userId);
        
        return _mapper.Map<List<PetSittingAdvertDto>>(adverts);
    }
    
    /// <summary>
    /// Updates an existing pet sitting request
    /// </summary>
    /// <param name="model">Data for updating the pet sitting request</param>
    /// <returns>The updated pet sitting advertisement DTO</returns>
    public async Task<PetSittingAdvertDto> UpdatePetSittingRequestAsync(UpdatePetSittingRequestViewModel model)
    {
        var advert = _mapper.Map<Advert>(model);
        
        var updatedAdvert = await _repository.UpdatePetSittingRequestAsync(advert, model.PetIds);
        
        return _mapper.Map<PetSittingAdvertDto>(updatedAdvert);
    }

    /// <summary>
    /// Updates an existing pet sitting offer
    /// </summary>
    /// <param name="model">Data for updating the pet sitting offer</param>
    /// <returns>The updated pet sitting advertisement DTO</returns>
    public async Task<PetSittingAdvertDto> UpdatePetSittingOfferAsync(UpdatePetSittingOfferViewModel model)
    {
        var advert = _mapper.Map<Advert>(model);
        
        var updatedAdvert = await _repository.UpdatePetSittingOfferAsync(advert, model.AcceptedAnimalTypeIds);
        
        return _mapper.Map<PetSittingAdvertDto>(updatedAdvert);
    }
    
    /// <summary>
    /// Deletes an advert
    /// </summary>
    /// <param name="advertId">The ID of the advert to delete</param>
    /// <returns>True if the deletion was successful, false if the advert wasn't found</returns>
    public async Task<bool> DeleteAdvertAsync(int advertId)
    {
        return await _repository.DeleteAdvertAsync(advertId);
    }
    
    public async Task<IEnumerable<PetSittingAdvertDto>> GetAdvertsWithSortingAsync(SortingOptions sortingOptions)
    {
        var adverts = await _repository.GetAdvertsWithSortingAsync(sortingOptions);
        return _mapper.Map<IEnumerable<PetSittingAdvertDto>>(adverts);
    }
    
    public async Task<AdvertViewModel> GetFilteredAdvertsAsync(bool isPetSitter, AdvertViewModel model)
    {
        // Récupérer toutes les annonces selon le type (offre ou demande)
        var allAdverts = await GetAllAdvertsAsync(isPetSitter);
        
        // Filtrer par prix
        var filteredAdverts = allAdverts
            .Where(a => a.Amount >= model.MinPrice && a.Amount <= model.MaxPrice);
        
        // Filtrer par type d'animal
        var animalType = allAdverts.SelectMany(a => a.PetCartViewModels)
            .Select(p => p.Species)
            .Distinct()
            .ToList();
        
        model.AnimalTypes = animalType.Select(name => new AnimalTypeAdvert
        {
            AnimalType = new AnimalType
            {
                Name = name
            }
        }).ToList();
        
        // Filter par prix
        model.MinPrice = model.MinPrice != 0 ? (int)Math.Round(model.MinPrice) : (int)Math.Round(allAdverts.Min(a => a.Amount));
        model.MaxPrice = model.MaxPrice != 0 ? (int)Math.Round(model.MaxPrice) : (int)Math.Round(allAdverts.Max(a => a.Amount));
        
        model.MinPriceBeforeReload = model.MinPriceBeforeReload != 0 ? (int)Math.Round(model.MinPriceBeforeReload) : (int)Math.Round(allAdverts.Min(a => a.Amount));
        model.MaxPriceBeforeReload = model.MaxPriceBeforeReload != 0 ? (int)Math.Round(model.MaxPriceBeforeReload) : (int)Math.Round(allAdverts.Max(a => a.Amount));
        
        // Appliquer les filtres de popularité
        var query = filteredAdverts.AsQueryable();

        // if (model.MostViewed)
        // {
        //     query = query.OrderByDescending(a => a.ViewCount);
        // }
        //
        // if (model.MostContracted)
        // {
        //     query = query.OrderByDescending(a => a.ContractCount);
        // }
        //
        // if (model.BestRated)
        // {
        //     query = query.OrderByDescending(a => a.Rating);
        // }

        // Appliquer les options de tri
        if (model.SortOptions != null)
        {
            switch (model.SortOptions.SortBy)
            {
                case "recent":
                    query = model.SortOptions.SortDirection == "asc" 
                        ? query.OrderBy(a => a.CreatedAt)
                        : query.OrderByDescending(a => a.CreatedAt);
                    break;
                case "oldest":
                    query = model.SortOptions.SortDirection == "asc"
                        ? query.OrderByDescending(a => a.CreatedAt) 
                        : query.OrderBy(a => a.CreatedAt);
                    break;
                case "soon":
                    query = model.SortOptions.SortDirection == "asc"
                        ? query.OrderBy(a => a.StartDate)
                        : query.OrderByDescending(a => a.StartDate);
                    break;
            }
        }
        
        // Appliquer le tri par prix
        if (model.MinPrice > 0)
        {
            query = query.Where(a => a.Amount >= model.MinPrice);
        }

        if (model.MaxPrice > 0 && model.MaxPrice < int.MaxValue)
        {
            query = query.Where(a => a.Amount <= model.MaxPrice);
        }
        
        // Appliquer le tri par type d'animal
        if (model.SelectedAnimalTypes != null && model.SelectedAnimalTypes.Length > 0)
        {
            query = query.Where(a => a.PetCartViewModels
                .Any(p => model.SelectedAnimalTypes.Contains(p.Species)));
        }
        
        // Mettre à jour le modèle avec les annonces filtrées et triées
        model.Adverts = query.ToList();
        
        // Retourner le modèle mis à jour
        return model;
    }
}