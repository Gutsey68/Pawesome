using Pawesome.Models.DTOs;

namespace Pawesome.Services.Interfaces;

public interface IPetSittingService
{
    Task<List<PetSittingAdvertDto>> GetAllAdvertsAsync(bool isPetSitter = false);
    Task<PetSittingAdvertDto?> GetAdvertByIdAsync(int id);
    Task<PetSittingAdvertDto> CreatePetSittingRequestAsync(PetSittingRequestDto dto, int userId);
    Task<PetSittingAdvertDto> CreatePetSittingOfferAsync(PetSittingOfferDto dto, int userId);
    Task<bool> UpdateAdvertStatusAsync(int advertId, string status);
    Task<List<PetSittingAdvertDto>> GetUserAdvertsAsync(int userId);
}