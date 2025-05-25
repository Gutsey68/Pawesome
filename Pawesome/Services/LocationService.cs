using Pawesome.Interfaces;
using Pawesome.Models.Entities;

namespace Pawesome.Services;

/// <summary>
/// Service for location-related operations
/// </summary>
public class LocationService : ILocationService
{
    private readonly ICityRepository _cityRepository;
    private readonly ICountryRepository _countryRepository;
    
    /// <summary>
    /// Initializes a new instance of the LocationService
    /// </summary>
    /// <param name="cityRepository">Repository for city operations</param>
    /// <param name="countryRepository">Repository for country operations</param>
    public LocationService(ICityRepository cityRepository, ICountryRepository countryRepository)
    {
        _cityRepository = cityRepository;
        _countryRepository = countryRepository;
    }
    
    /// <summary>
    /// Gets all cities
    /// </summary>
    /// <returns>List of all cities</returns>
    public async Task<List<City>> GetAllCitiesAsync()
    {
        return (List<City>)await _cityRepository.GetAllAsync();
    }
    
    /// <summary>
    /// Gets all cities in a specific country
    /// </summary>
    /// <param name="countryId">ID of the country</param>
    /// <returns>List of cities in the country</returns>
    public async Task<List<City>> GetCitiesByCountryIdAsync(int countryId)
    {
        var allCities = await _cityRepository.GetAllAsync();
        return allCities.Where(c => c.CountryId == countryId).ToList();
    }
    
    /// <summary>
    /// Gets all countries
    /// </summary>
    /// <returns>List of all countries</returns>
    public async Task<List<Country>> GetAllCountriesAsync()
    {
        return (List<Country>)await _countryRepository.GetAllAsync();
    }
    
    /// <summary>
    /// Gets a city by its ID
    /// </summary>
    /// <param name="cityId">ID of the city</param>
    /// <returns>The city if found, null otherwise</returns>
    public async Task<City?> GetCityByIdAsync(int cityId)
    {
        return await _cityRepository.GetByIdAsync(cityId);
    }
    
    /// <summary>
    /// Gets a city by its name and postal code
    /// </summary>
    /// <param name="cityName">Name of the city</param>
    /// <param name="postalCode">Postal code of the city</param>
    /// <returns>The city if found, null otherwise</returns>
    public async Task<City?> GetCityByNameAndPostalCodeAsync(string cityName, string postalCode)
    {
        return await _cityRepository.GetByNameAndPostalCodeAsync(cityName, postalCode);
    }
}
