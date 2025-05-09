using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.ViewModels.Advert;

namespace Pawesome.Interfaces;

public interface IAdvertService
{
    Task<List<PetSittingAdvertDto>> GetAllAdvertsAsync(bool isPetSitter = false);
    Task<PetSittingAdvertDto?> GetAdvertByIdAsync(int id);
    Task<PetSittingAdvertDto> CreatePetSittingRequestAsync(PetSittingRequestViewModel model, int userId);
    Task<PetSittingAdvertDto> CreatePetSittingOfferAsync(PetSittingOfferViewModel model, int userId);
    Task<bool> UpdateAdvertStatusAsync(int advertId, string status);
    Task<List<PetSittingAdvertDto>> GetUserAdvertsAsync(int userId);
    Task<PetSittingAdvertDto> UpdatePetSittingRequestAsync(UpdatePetSittingRequestViewModel model);
    Task<PetSittingAdvertDto> UpdatePetSittingOfferAsync(UpdatePetSittingOfferViewModel model);
    Task<bool> DeleteAdvertAsync(int advertId);
    Task<AdvertViewModel> GetFilteredAdvertsAsync(bool isPetSitter, AdvertViewModel model);
}