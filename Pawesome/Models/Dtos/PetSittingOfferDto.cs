namespace Pawesome.Models.DTOs;

public class PetSittingOfferDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public List<int> AcceptedAnimalTypeIds { get; set; } = new();
    public string? AdditionalInformation { get; set; }
}