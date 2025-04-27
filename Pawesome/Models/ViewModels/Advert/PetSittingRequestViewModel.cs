namespace Pawesome.Models.ViewModels.Advert;

public class PetSittingRequestViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public List<int> PetIds { get; set; } = new();
    public string? AdditionalInformation { get; set; }
}