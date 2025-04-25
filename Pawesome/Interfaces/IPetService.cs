using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Pet;
using Pawesome.Models.ViewModels;

namespace Pawesome.Interfaces;

public interface IPetService
{
    Task<IEnumerable<PetViewModel>> GetUserPetsAsync(int userId);
    Task<PetDetailsViewModel?> GetPetDetailsAsync(int id);
    Task<int> CreatePetAsync(CreatePetDto petDto, int userId);
    Task UpdatePetAsync(UpdatePetDto petDto);
    Task DeletePetAsync(int id);
    Task<IEnumerable<AnimalTypeViewModel>> GetAnimalTypesAsync();
    Task<UpdatePetDto?> GetPetForEditAsync(int id);
    Task<List<PetViewModel>> GetUserPets(int userId);
    
}