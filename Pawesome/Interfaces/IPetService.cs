using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.AnimalType;
using Pawesome.Models.ViewModels.Pet;

namespace Pawesome.Interfaces;

public interface IPetService
{
    Task<IEnumerable<PetViewModel>> GetUserPetsAsync(int userId);
    Task<int> CreatePetAsync(CreatePetViewModel model, int userId);
    Task DeletePetAsync(int id);
    Task<IEnumerable<AnimalTypeViewModel>> GetAnimalTypesAsync();
    Task<List<PetViewModel>> GetUserPets(int userId);
    Task<UpdatePetViewModel?> GetPetForEditAsync(int id);
    Task UpdatePetAsync(UpdatePetViewModel model);
    Task<PetDetailsViewModel?> GetPetDetailsAsync(int id);
    public Task<int> GetPetsCountAsync();

}