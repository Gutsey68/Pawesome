using AutoMapper;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Entities;
using Pawesome.Models.Enums;
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
            Status = AdvertStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PetAdverts = new List<PetAdvert>(),
            AnimalTypeAdverts = new List<AnimalTypeAdvert>(),
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
            Status = AdvertStatus.PendingOffer,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PetAdverts = new List<PetAdvert>(),
            AnimalTypeAdverts = new List<AnimalTypeAdvert>(),
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
    public async Task<bool> UpdateAdvertStatusAsync(int advertId, AdvertStatus status)
    {
        return await _repository.UpdateAdvertStatusAsync(advertId, status);
    }

    /// <summary>
    /// Retrieves all adverts associated with a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose adverts to retrieve</param>
    /// <param name="includeCancelled">Whether to include cancelled adverts</param>
    /// <returns>A list of pet sitting advertisement DTOs belonging to the specified user</returns>
    public async Task<List<PetSittingAdvertDto>> GetUserAdvertsAsync(int userId, bool includeCancelled = true)
    {
        var adverts = await _repository.GetUserAdvertsAsync(userId, includeCancelled);

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
    /// Retrieves filtered adverts based on the provided filter criteria
    /// </summary>
    /// <param name="filterDto">Filter criteria for retrieving adverts</param>
    /// <returns>A list of filtered pet sitting advertisement DTOs</returns>
    public async Task<List<PetSittingAdvertDto>> GetFilteredAdvertsAsync(AdvertFilterDto filterDto)
    {
        var adverts = await _repository.GetFilteredAdvertsAsync(filterDto);
        return _mapper.Map<List<PetSittingAdvertDto>>(adverts);
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

    /// <summary>
    /// Returns the total number of adverts in the system.
    /// </summary>
    /// <returns>The count of all adverts.</returns>
    public async Task<int> GetAdvertsCountAsync()
    {
        return await _repository.GetAdvertsCountAsync();
    }

    /// <summary>
    /// Retrieves all adverts as a list of AdvertDto.
    /// </summary>
    /// <returns>A list of AdvertDto representing all adverts in the system.</returns>
    public List<AdvertDto> GetAllAdverts()
    {
        var adverts = _repository.GetAllAdvertsWithUsers();
        return _mapper.Map<List<AdvertDto>>(adverts);
    }
}
