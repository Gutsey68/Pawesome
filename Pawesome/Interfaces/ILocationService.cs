using Pawesome.Models.Entities;

namespace Pawesome.Interfaces;

/// <summary>
/// Interface for location-related operations
/// </summary>
public interface ILocationService
{
    Task<List<City>> GetAllCitiesAsync();
    Task<List<City>> GetCitiesByCountryIdAsync(int countryId);
    Task<List<Country>> GetAllCountriesAsync();
    Task<City?> GetCityByIdAsync(int cityId);
    Task<City?> GetCityByNameAndPostalCodeAsync(string cityName, string postalCode);
}
