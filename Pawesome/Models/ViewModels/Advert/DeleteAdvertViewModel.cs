namespace Pawesome.Models.ViewModels.Advert
{
    public class DeleteAdvertViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsPetSitter { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}