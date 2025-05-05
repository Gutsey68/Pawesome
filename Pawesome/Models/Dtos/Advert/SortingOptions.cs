namespace Pawesome.Models.Dtos.Advert;

public class SortingOptions
{
    public string SortBy { get; set; } = "Near"; // Recent, Last, Near
    
    public decimal? StartPrice { get; set; }
    public decimal? EndPrice { get; set; }
    
    public bool? MostViewed { get; set; }
    public bool? MostContracted { get; set; }
    public bool? BestRated { get; set; }
    
    public string SortDirection { get; set; } = "desc"; // asc, desc
    
    public bool? VerifiedProfile { get; set; }
    public bool? BestRating { get; set; }
    
}