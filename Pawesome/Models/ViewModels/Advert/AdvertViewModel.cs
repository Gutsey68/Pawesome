using Pawesome.Models.Dtos.Advert;

namespace Pawesome.Models.ViewModels.Advert;

public class AdvertViewModel
{
    // Liste des annonces affichées sur la page
    public required IEnumerable<PetSittingAdvertDto> Adverts { get; set; }

    // Options pour appliquer le tri
    public required SortingOptions SortingOptions { get; set; }
}