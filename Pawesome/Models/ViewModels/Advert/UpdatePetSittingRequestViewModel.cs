namespace Pawesome.Models.ViewModels.Advert
{
    public class UpdatePetSittingRequestViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public List<int> PetIds { get; set; } = new List<int>();
        public string? AdditionalInformation { get; set; }
    }
}