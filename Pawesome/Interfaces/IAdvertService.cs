using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Advert;

namespace Pawesome.Interfaces;

public interface IAdvertService
{
    Task<List<PetSittingAdvertDto>> GetAllAdvertsAsync(bool isPetSitter = false);
    Task<PetSittingAdvertDto?> GetAdvertByIdAsync(int id);
    Task<PetSittingAdvertDto> CreatePetSittingRequestAsync(PetSittingRequestDto dto, int userId);
    Task<PetSittingAdvertDto> CreatePetSittingOfferAsync(PetSittingOfferDto dto, int userId);
    Task<bool> UpdateAdvertStatusAsync(int advertId, string status);
    Task<List<PetSittingAdvertDto>> GetUserAdvertsAsync(int userId);
}