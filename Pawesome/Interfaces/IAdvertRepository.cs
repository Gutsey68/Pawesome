using Pawesome.Models.Entities;
using Pawesome.Models.Dtos.Advert;

namespace Pawesome.Interfaces
{
    public interface IAdvertRepository
    {
        Task<Advert?> GetAdvertByIdAsync(int id);
        Task<List<Advert>> GetAllAdvertsAsync(bool isPetSitter = false);
        Task<List<Advert>> GetUserAdvertsAsync(int userId);
        Task<Advert> CreatePetSittingRequestAsync(Advert advert, List<int> petIds, int userId);
        Task<Advert> CreatePetSittingOfferAsync(Advert advert, List<int> animalTypeIds, int userId);
        Task<Advert> UpdatePetSittingRequestAsync(Advert advert, List<int> petIds);
        Task<Advert> UpdatePetSittingOfferAsync(Advert advert, List<int> animalTypeIds);
        Task<bool> UpdateAdvertStatusAsync(int advertId, string status);
        Task<bool> DeleteAdvertAsync(int advertId);
        Task<List<Advert>> GetFilteredAdvertsAsync(AdvertFilterDto filter);
    }
}
