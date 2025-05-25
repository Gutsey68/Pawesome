namespace Pawesome.Models.Dtos.Advert
{
    /// <summary>
    /// Data transfer object for filtering adverts
    /// </summary>
    public class AdvertFilterDto
    {
        public bool? IsPetSitterOffer { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public List<int>? AnimalTypeIds { get; set; }
        public int? CityId { get; set; }
        public int? CountryId { get; set; }
        public string? PostalCode { get; set; }
        public DateTime? CreatedAtFrom { get; set; }
        public DateTime? CreatedAtTo { get; set; }
        public string? City { get; set; }
    }
}
