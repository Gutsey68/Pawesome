using Pawesome.Models;

namespace Pawesome.Interfaces;

public interface IAdvertRepository
{
    Task<List<Advert>> GetAllAdvertsAsync(bool isPetSitter = false);
    Task<Advert?> GetAdvertByIdAsync(int id);
    Task<Advert> CreatePetSittingRequestAsync(Advert advert, List<int> petIds);
    Task<Advert> CreatePetSittingOfferAsync(Advert advert, List<int> animalTypeIds);
    Task<bool> UpdateAdvertStatusAsync(int advertId, string status);
    Task<List<Advert>> GetUserAdvertsAsync(int userId);
}