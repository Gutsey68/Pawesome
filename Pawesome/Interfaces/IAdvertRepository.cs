using Pawesome.Models;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

public interface IAdvertRepository
{
    Task<List<Advert>> GetAllAdvertsAsync(bool isPetSitter = false);
    Task<Advert?> GetAdvertByIdAsync(int id);
    Task<Advert> CreatePetSittingRequestAsync(Advert advert, List<int> petIds, int userId);
    Task<Advert> CreatePetSittingOfferAsync(Advert advert, List<int> animalTypeIds, int userId);
    Task<bool> UpdateAdvertStatusAsync(int advertId, string status);
    Task<List<Advert>> GetUserAdvertsAsync(int userId);
    Task<Advert> UpdatePetSittingRequestAsync(Advert advert, List<int> petIds);
    Task<Advert> UpdatePetSittingOfferAsync(Advert advert, List<int> animalTypeIds);
    Task<bool> DeleteAdvertAsync(int advertId);
    
    Task<IEnumerable<Advert>> GetAdvertsWithSortingAsync(SortingOptions sortingOptions);
}