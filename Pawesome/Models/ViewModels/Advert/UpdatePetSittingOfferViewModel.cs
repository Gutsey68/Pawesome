namespace Pawesome.Models.ViewModels.Advert
{
    public class UpdatePetSittingOfferViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public List<int> AcceptedAnimalTypeIds { get; set; } = new List<int>();
        public string? AdditionalInformation { get; set; }
    }
}