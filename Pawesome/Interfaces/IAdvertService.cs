using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Enums;
using Pawesome.Models.ViewModels.Advert;

namespace Pawesome.Interfaces;

public interface IAdvertService
{
    Task<List<PetSittingAdvertDto>> GetAllAdvertsAsync(bool isPetSitter = false);
    Task<PetSittingAdvertDto?> GetAdvertByIdAsync(int id);
    Task<PetSittingAdvertDto> CreatePetSittingRequestAsync(PetSittingRequestViewModel model, int userId);
    Task<PetSittingAdvertDto> CreatePetSittingOfferAsync(PetSittingOfferViewModel model, int userId);
    Task<bool> UpdateAdvertStatusAsync(int advertId, AdvertStatus status);
    Task<List<PetSittingAdvertDto>> GetUserAdvertsAsync(int userId, bool includeCancelled = true);
    Task<PetSittingAdvertDto> UpdatePetSittingRequestAsync(UpdatePetSittingRequestViewModel model);
    Task<PetSittingAdvertDto> UpdatePetSittingOfferAsync(UpdatePetSittingOfferViewModel model);
    Task<bool> DeleteAdvertAsync(int advertId);
    Task<List<PetSittingAdvertDto>> GetFilteredAdvertsAsync(AdvertFilterDto filter);
    Task<int> GetAdvertsCountAsync();
    public List<AdvertDto> GetAllAdverts();
}
