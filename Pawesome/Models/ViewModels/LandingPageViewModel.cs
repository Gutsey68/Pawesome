using System.Collections;

namespace Pawesome.Models.ViewModels;

public class LandingPageViewModel
{
    public required List<PetCartLandingViewModel> PetCards { get; set; }
    public required List<CommentCardLandingViewModel> CommentCards { get; set; }
}
