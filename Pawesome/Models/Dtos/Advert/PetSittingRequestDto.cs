namespace Pawesome.Models.Dtos.Advert;

public class PetSittingRequestDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public List<int> PetIds { get; set; } = new();
    public string? AdditionalInformation { get; set; }
}