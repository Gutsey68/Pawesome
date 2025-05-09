using Pawesome.Interfaces;
using Pawesome.Models.Dtos.Advert;

namespace Pawesome.Models.ViewModels.Advert;

public class AdvertViewModel
{
    // Liste des annonces affichées sur la page
    public required IEnumerable<PetSittingAdvertDto> Adverts { get; set; }

    public bool IsPetSitter { get; set; }
    // Options pour appliquer le tri
    
    // Liste des annonces par prix
    public decimal MinPrice { get; set; }
    public decimal MinPriceBeforeReload { get; set; }
    
    public decimal MaxPrice { get; set; }
    public decimal MaxPriceBeforeReload { get; set; }
    
    // Liste des animaux de compagnie
    public required List<AnimalTypeAdvert> AnimalTypes { get; set; }
    public string[]? SelectedAnimalTypes { get; set; }
    
    // Liste des annonces les plus vues
    public bool MostViewed { get; set; }
    public int ViewCountTotal { get; set; }
    public bool MostContracted { get; set; }
    public int ContractCountTotal { get; set; }
    public bool BestRated { get; set; }
    public int RatingCountTotal { get; set; }
    
    // Liste des utilisateurs vérifiés
    public bool VerifiedUsers { get; set; }
    public int VerifiedUsersCount { get; set; }
    
    // Liste des utilisateurs les mieux notés
    public bool BestRatedUsers { get; set; }
    public int BestRatedUsersCount { get; set; }

    public required IAdvertSortingOptions SortOptions { get; set; }
}