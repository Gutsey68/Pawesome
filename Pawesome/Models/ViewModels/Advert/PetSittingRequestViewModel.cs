namespace Pawesome.Models.ViewModels.Advert;

public class PetSittingRequestViewModel
{
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);
    public decimal Amount { get; set; }
    public List<int> PetIds { get; set; } = new();
    public string? AdditionalInformation { get; set; }
}