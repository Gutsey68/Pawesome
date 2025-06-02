using Pawesome.Models.Entities;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Enums;

namespace Pawesome.Interfaces
{
    public interface IAdvertRepository
    {
        Task<Advert?> GetAdvertByIdAsync(int id);
        Task<List<Advert>> GetAllAdvertsAsync(bool isPetSitter = false);
        Task<List<Advert>> GetUserAdvertsAsync(int userId, bool includeCancelled = true);
        Task<Advert> CreatePetSittingRequestAsync(Advert advert, List<int> petIds, int userId);
        Task<Advert> CreatePetSittingOfferAsync(Advert advert, List<int> animalTypeIds, int userId);
        Task<Advert> UpdatePetSittingRequestAsync(Advert advert, List<int> petIds);
        Task<Advert> UpdatePetSittingOfferAsync(Advert advert, List<int> animalTypeIds);
        Task<bool> UpdateAdvertStatusAsync(int advertId, AdvertStatus status);
        Task<bool> DeleteAdvertAsync(int advertId);
        Task<IEnumerable<Advert>> GetFilteredAdvertsAsync(AdvertFilterDto filter);
        public List<Advert> GetAllAdvertsWithUsers();
        Task<int> GetAdvertsCountAsync();
    }
}
