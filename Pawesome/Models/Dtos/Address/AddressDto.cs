namespace Pawesome.Models.DTOs.Address;

public class AddressDto
{
    public int Id { get; set; }
    public string StreetAddress { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
}