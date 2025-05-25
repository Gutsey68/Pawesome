using AutoMapper;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.ViewModels;
using Pawesome.Models.ViewModels.AnimalType;

namespace Pawesome.Services;

/// <summary>
/// Service handling animal type operations
/// </summary>
public class AnimalTypeService : IAnimalTypeService
{
    private readonly IAnimalTypeRepository _repository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the AnimalTypeService
    /// </summary>
    /// <param name="repository">Repository for animal type data operations</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    public AnimalTypeService(IAnimalTypeRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all animal types from the database
    /// </summary>
    /// <returns>A list of animal types as view models</returns>
    public async Task<List<AnimalTypeViewModel>> GetAllAnimalTypesAsync()
    {
        var animalTypes = await _repository.GetAllAnimalTypesAsync();
        return _mapper.Map<List<AnimalTypeViewModel>>(animalTypes);
    }

    /// <summary>
    /// Retrieves a specific animal type by its ID
    /// </summary>
    /// <param name="id">The ID of the animal type to retrieve</param>
    /// <returns>The animal type view model if found, null otherwise</returns>
    public async Task<AnimalTypeViewModel?> GetAnimalTypeByIdAsync(int id)
    {
        var animalType = await _repository.GetAnimalTypeByIdAsync(id);
        return animalType != null ? _mapper.Map<AnimalTypeViewModel>(animalType) : null;
    }
}