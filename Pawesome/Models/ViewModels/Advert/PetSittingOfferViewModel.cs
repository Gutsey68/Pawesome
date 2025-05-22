namespace Pawesome.Models.ViewModels.Advert;

public class PetSittingOfferViewModel
{
    public DateTime StartDate { get; set; } = DateTime.Today.AddDays(1);
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(2);
    public decimal Amount { get; set; }
    public List<int> AcceptedAnimalTypeIds { get; set; } = new();
    public string? AdditionalInformation { get; set; }
}