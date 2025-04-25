using AutoMapper;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Pet;
using Pawesome.Models.ViewModels;

namespace Pawesome.Services;

/// <summary>
/// Service handling business logic for pet-related operations
/// </summary>
public class PetService : IPetService
{
    private readonly IPetRepository _petRepository;
    private readonly IAnimalTypeRepository _animalTypeRepository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the PetService
    /// </summary>
    /// <param name="petRepository">Repository for pet operations</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    /// <param name="environment">Web host environment for file operations</param>
    /// <param name="animalTypeRepository">Repository for animal type operations</param>
    public PetService(IPetRepository petRepository, IMapper mapper, IWebHostEnvironment environment, IAnimalTypeRepository animalTypeRepository)
    {
        _petRepository = petRepository;
        _animalTypeRepository = animalTypeRepository;
        _mapper = mapper;
        _environment = environment;
    }

    /// <summary>
    /// Retrieves all pets for a specific user
    /// </summary>
    /// <param name="userId">The ID of the user whose pets to retrieve</param>
    /// <returns>Collection of pet view models</returns>
    public async Task<IEnumerable<PetViewModel>> GetUserPetsAsync(int userId)
    {
        var pets = await _petRepository.GetPetsByUserIdAsync(userId);
        
        return _mapper.Map<IEnumerable<PetViewModel>>(pets);
    }

    /// <summary>
    /// Retrieves detailed information about a specific pet
    /// </summary>
    /// <param name="id">The ID of the pet</param>
    /// <returns>Pet details view model if found, null otherwise</returns>
    public async Task<PetDetailsViewModel?> GetPetDetailsAsync(int id)
    {
        var pet = await _petRepository.GetPetWithDetailsAsync(id);
        
        return pet != null ? _mapper.Map<PetDetailsViewModel>(pet) : null;
    }

    /// <summary>
    /// Creates a new pet for a user
    /// </summary>
    /// <param name="petDto">The pet creation data</param>
    /// <param name="userId">The ID of the user who owns the pet</param>
    /// <returns>The ID of the created pet</returns>
    public async Task<int> CreatePetAsync(CreatePetDto petDto, int userId)
    {
        var pet = _mapper.Map<Pet>(petDto);
        
        pet.UserId = userId;
        
        if (petDto.Photo != null)
        {
            var fileName = await SavePhotoAsync(petDto.Photo);
            pet.Photo = fileName;
        }
        
        var result = await _petRepository.AddAsync(pet);
        
        await _petRepository.SaveChangesAsync();
        
        return result.Id;
    }

    /// <summary>
    /// Updates an existing pet's information
    /// </summary>
    /// <param name="petDto">The updated pet data</param>
    public async Task UpdatePetAsync(UpdatePetDto petDto)
    {
        var pet = await _petRepository.GetByIdAsync(petDto.Id);

        if (pet != null)
        {
            var oldPhoto = pet.Photo;

            _mapper.Map(petDto, pet);

            if (petDto.Photo != null && petDto.Photo.Length > 0)
            {
                pet.Photo = await SavePhotoAsync(petDto.Photo);

                if (!string.IsNullOrEmpty(oldPhoto))
                {
                    DeletePhoto(oldPhoto);
                }
            }
            else
            {
                pet.Photo = oldPhoto;
            }

            await _petRepository.UpdateAsync(pet);
            
            await _petRepository.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Deletes a pet and its associated photo
    /// </summary>
    /// <param name="id">The ID of the pet to delete</param>
    public async Task DeletePetAsync(int id)
    {
        var pet = await _petRepository.GetByIdAsync(id);
        
        if (pet != null)
        {
            if (!string.IsNullOrEmpty(pet.Photo))
            {
                DeletePhoto(pet.Photo);
            }
            
            await _petRepository.DeleteAsync(pet);
            
            await _petRepository.SaveChangesAsync();
        }
    }
    
    /// <summary>
    /// Saves a pet photo to the file system
    /// </summary>
    /// <param name="photo">The photo file to save</param>
    /// <returns>The generated filename</returns>
    private async Task<string> SavePhotoAsync(IFormFile photo)
    {
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName)}";
        
        var filePath = Path.Combine(_environment.WebRootPath, "images", "pets", fileName);
        
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await using var stream = new FileStream(filePath, FileMode.Create);
        
        await photo.CopyToAsync(stream);

        return fileName;
    }
    
    /// <summary>
    /// Deletes a pet photo from the file system
    /// </summary>
    /// <param name="fileName">The name of the file to delete</param>
    private void DeletePhoto(string fileName)
    {
        var filePath = Path.Combine(_environment.WebRootPath, "images", "pets", fileName);
        
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
    
    /// <summary>
    /// Retrieves all available animal types
    /// </summary>
    /// <returns>Collection of animal type view models</returns>
    public async Task<IEnumerable<AnimalTypeViewModel>> GetAnimalTypesAsync()
    {
        var animalTypes = await _animalTypeRepository.GetAllAnimalTypesAsync();
        
        return _mapper.Map<IEnumerable<AnimalTypeViewModel>>(animalTypes);
    }

    /// <summary>
    /// Retrieves pet data for editing
    /// </summary>
    /// <param name="id">The ID of the pet to edit</param>
    /// <returns>Update pet DTO if found, null otherwise</returns>
    public async Task<UpdatePetDto?> GetPetForEditAsync(int id)
    {
        var pet = await _petRepository.GetPetWithDetailsAsync(id);
        
        if (pet == null) return null;
        
        var updatePetDto = _mapper.Map<UpdatePetDto>(pet);
    
        return updatePetDto;
    }

    public Task<List<PetViewModel>> GetUserPets(int userId)
    {
        return _petRepository.GetPetsByUserIdAsync(userId)
            .ContinueWith(task => _mapper.Map<List<PetViewModel>>(task.Result));
    }
}