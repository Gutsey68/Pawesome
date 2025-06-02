using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pawesome.Interfaces;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Entities;
using Pawesome.Models.Enums;
using Pawesome.Models.ViewModels.Advert;
using Pawesome.Models.ViewModels.AnimalType;

namespace Pawesome.Controllers;

/// <summary>
/// Controller for managing pet sitting offers and requests
/// </summary>
public class AdvertController : Controller
{
    private readonly IAdvertService _advertService;
    private readonly IPetService _petService;
    private readonly IBookingService _bookingService;
    private readonly IAnimalTypeService _animalTypeService;
    private readonly ILocationService _locationService;
    private readonly UserManager<User> _userManager;

    public AdvertController(
        IAdvertService advertService,
        IPetService petService,
        IBookingService bookingService,
        IAnimalTypeService animalTypeService,
        ILocationService locationService,
        UserManager<User> userManager)
    {
        _advertService = advertService;
        _petService = petService;
        _bookingService = bookingService;
        _animalTypeService = animalTypeService;
        _locationService = locationService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(bool isPetSitter = false, [FromQuery] AdvertViewModel? model = null)
    {
        if (model == null)
        {
            model = new AdvertViewModel();
        }

        if (!model.IsPetSitterOffer.HasValue)
        {
            model.IsPetSitterOffer = isPetSitter;
        }

        try
        {
            var animalTypes = await _animalTypeService.GetAllAnimalTypesAsync();

            model.AnimalTypeOptions = animalTypes
                .Where(at => at?.AnimalType != null)
                .Select(at => new SelectListItem
                {
                    Value = at.AnimalType.Id.ToString(),
                    Text = at.AnimalType.Name ?? "Type inconnu",
                    Selected = model.AnimalTypeIds != null && model.AnimalTypeIds.Contains(at.AnimalType.Id)
                })
                .ToList();

            model.AnimalTypes = animalTypes;

            if (model.AnimalTypeIds != null && model.AnimalTypeIds.Count != 0)
            {
                model.SelectedAnimalTypes = animalTypes
                    .Where(at => at?.AnimalType != null && model.AnimalTypeIds.Contains(at.AnimalType.Id))
                    .Select(at => at.AnimalType.Name)
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la récupération des types d'animaux: {ex.Message}");
            model.AnimalTypeOptions = new List<SelectListItem>();
            model.AnimalTypes = new List<AnimalTypeViewModel>();
        }

        var countries = await _locationService.GetAllCountriesAsync();
        model.CountryOptions = countries.Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.Name,
            Selected = model.CountryId.HasValue && c.Id == model.CountryId.Value
        }).ToList();

        var allAdvertsFilter = new AdvertFilterDto
        {
            IsPetSitterOffer = model.IsPetSitterOffer
        };
        var allAdverts = await _advertService.GetFilteredAdvertsAsync(allAdvertsFilter);
        var allPrices = allAdverts.Select(a => a.Amount).ToList();

        if (allPrices.Any())
        {
            model.MinPriceBeforeReload = Math.Floor(allPrices.Min());
            model.MaxPriceBeforeReload = Math.Ceiling(allPrices.Max());
            
            if (!model.MinPrice.HasValue)
                model.MinPrice = model.MinPriceBeforeReload;
            else
                model.MinPrice = Math.Round(model.MinPrice.Value);
                
            if (!model.MaxPrice.HasValue)
                model.MaxPrice = model.MaxPriceBeforeReload;
            else
                model.MaxPrice = Math.Round(model.MaxPrice.Value);
        }
        
        var filterDto = new AdvertFilterDto
        {
            IsPetSitterOffer = model.IsPetSitterOffer,
            MinPrice = model.MinPrice,
            MaxPrice = model.MaxPrice,
            StartDateFrom = model.StartDateFrom,
            EndDateTo = model.EndDateTo,
            AnimalTypeIds = model.AnimalTypeIds,
            CountryId = model.CountryId,
            City = model.City,
            CreatedAtFrom = model.CreatedAtFrom,
            CreatedAtTo = model.CreatedAtTo
        };

        var adverts = await _advertService.GetFilteredAdvertsAsync(filterDto);

        adverts = model.SortOption switch
        {
            "oldest" => adverts.OrderBy(a => a.CreatedAt).ToList(),
            "price_asc" => adverts.OrderBy(a => a.Amount).ToList(),
            "price_desc" => adverts.OrderByDescending(a => a.Amount).ToList(),
            "date_start_asc" => adverts.OrderBy(a => a.StartDate).ToList(),
            "date_end_asc" => adverts.OrderBy(a => a.EndDate).ToList(),
            _ => adverts.OrderByDescending(a => a.CreatedAt).ToList()
        };

        model.Adverts = adverts;

        foreach (var option in model.SortOptions)
        {
            option.Selected = option.Value == model.SortOption;
        }

        return View(model);
    }

    /// <summary>
    /// Displays details for a specific advert
    /// </summary>
    /// <param name="id">ID of the advert to display</param>
    /// <returns>View containing advert details or NotFound if not found</returns>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var advert = await _advertService.GetAdvertByIdAsync(id);

        if (advert == null)
        {
            return NotFound();
        }

        return View(advert);
    }

    /// <summary>
    /// Displays the form for creating a new pet sitting request
    /// </summary>
    /// <returns>View containing the creation form with user's pets</returns>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> CreateRequest()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        var pets = await _petService.GetUserPets(user.Id);

        ViewBag.Pets = pets;
        return View(new PetSittingRequestViewModel());
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateRequest(PetSittingRequestViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }
            
            if (user.Address == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Vous devez définir votre localisation dans votre profil avant de pouvoir créer une annonce.");
            }

            var pets = await _petService.GetUserPets(user.Id);
            
            if (pets.Count == 0)
            {
                ModelState.AddModelError(string.Empty,
                    "Vous devez avoir au moins un animal enregistré pour créer une demande de pet sitting.");
            }
            
            ViewBag.Pets = pets;

            return View(viewModel);
        }

        var existingUser = await _userManager.GetUserAsync(User);

        if (existingUser == null)
        {
            return Challenge();
        }

        var result = await _advertService.CreatePetSittingRequestAsync(viewModel, existingUser.Id);
        return RedirectToAction(nameof(Details), new { id = result.Id });
    }

    /// <summary>
    /// Displays the form for creating a new pet sitting offer
    /// </summary>
    /// <returns>View containing the creation form with animal types</returns>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> CreateOffer()
    {
        var user = await _userManager.GetUserAsync(User);
    
        if (user == null)
        {
            return Challenge();
        }
    
        var animalTypes = await _animalTypeService.GetAllAnimalTypesAsync();
        ViewBag.AnimalTypes = animalTypes;

        return View(new PetSittingOfferViewModel());
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateOffer(PetSittingOfferViewModel viewModel)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }
        
        if (!ModelState.IsValid)
        {
            if (user.Address == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Vous devez définir votre localisation dans votre profil avant de pouvoir créer une annonce.");
            }
            
            var animalTypes = await _animalTypeService.GetAllAnimalTypesAsync();
            ViewBag.AnimalTypes = animalTypes;

            return View(viewModel);
        }

        var result = await _advertService.CreatePetSittingOfferAsync(viewModel, user.Id);

        return RedirectToAction(nameof(Details), new { id = result.Id });
    }

    /// <summary>
    /// Updates the status of an existing advert
    /// </summary>
    /// <param name="advertId">ID of the advert to update</param>
    /// <param name="status">New status value</param>
    /// <returns>Redirects to Details if successful, or returns NotFound if advert not found</returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int advertId, AdvertStatus status)
    {
        var result = await _advertService.UpdateAdvertStatusAsync(advertId, status);

        if (!result)
        {
            return NotFound();
        }

        return status == AdvertStatus.Cancelled
            ? RedirectToAction("Index", "Advert")
            : RedirectToAction(nameof(Details), new { id = advertId });
    }

    /// <summary>
    /// Displays a list of the current user's adverts
    /// </summary>
    /// <returns>View containing user's adverts</returns>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> MyAdverts()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Challenge();
        
        var adverts = await _advertService.GetUserAdvertsAsync(user.Id);
    
        ViewBag.PendingBookings = await _bookingService.GetPendingBookingsForUserAdvertsAsync(user.Id);
    
        return View(adverts);
    }

    /// <summary>
    /// Displays the form for editing a pet sitting request
    /// </summary>
    /// <param name="id">ID of the pet sitting request to edit</param>
    /// <returns>View containing the edit form with user's pets</returns>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> EditRequest(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        var advert = await _advertService.GetAdvertByIdAsync(id);

        if (advert == null)
        {
            return NotFound();
        }

        if (advert.Owner.Id != user.Id)
        {
            return Forbid();
        }

        var updateModel = new UpdatePetSittingRequestViewModel
        {
            Id = advert.Id,
            StartDate = advert.StartDate,
            EndDate = advert.EndDate,
            Amount = advert.Amount,
            PetIds = advert.Pets.Select(p => p.Id).ToList(),
            AdditionalInformation = advert.AdditionalInformation
        };

        var pets = await _petService.GetUserPets(user.Id);
        ViewBag.Pets = pets;

        return View(updateModel);
    }

    /// <summary>
    /// Processes the update of a pet sitting request
    /// </summary>
    /// <param name="viewModel">Updated data for the pet sitting request</param>
    /// <returns>Redirects to Details if successful, or returns the form with validation errors</returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditRequest(UpdatePetSittingRequestViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge();
            }

            var pets = await _petService.GetUserPets(user.Id);
            ViewBag.Pets = pets;

            return View(viewModel);
        }

        var result = await _advertService.UpdatePetSittingRequestAsync(viewModel);

        return RedirectToAction(nameof(Details), new { id = result.Id });
    }

    /// <summary>
    /// Displays the form for editing a pet sitting offer
    /// </summary>
    /// <param name="id">ID of the pet sitting offer to edit</param>
    /// <returns>View containing the edit form with animal types</returns>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> EditOffer(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Challenge();
        }

        var advert = await _advertService.GetAdvertByIdAsync(id);

        if (advert == null)
        {
            return NotFound();
        }

        if (advert.Owner.Id != user.Id)
        {
            return Forbid();
        }

        var updateModel = new UpdatePetSittingOfferViewModel
        {
            Id = advert.Id,
            StartDate = advert.StartDate,
            EndDate = advert.EndDate,
            Amount = advert.Amount,
            AcceptedAnimalTypeIds = new List<int>(),
            AdditionalInformation = advert.AdditionalInformation
        };

        var animalTypes = await _animalTypeService.GetAllAnimalTypesAsync();
        ViewBag.AnimalTypes = animalTypes;

        foreach (var animalType in animalTypes)
        {
            if (advert.Pets.Any(p => p.Name == animalType.AnimalType.Name))
            {
                updateModel.AcceptedAnimalTypeIds.Add(animalType.AnimalType.Id);
            }
        }

        return View(updateModel);
    }

    /// <summary>
    /// Processes the update of a pet sitting offer
    /// </summary>
    /// <param name="viewModel">Updated data for the pet sitting offer</param>
    /// <returns>Redirects to Details if successful, or returns the form with validation errors</returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditOffer(UpdatePetSittingOfferViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var animalTypes = await _animalTypeService.GetAllAnimalTypesAsync();
            ViewBag.AnimalTypes = animalTypes;

            return View(viewModel);
        }

        var result = await _advertService.UpdatePetSittingOfferAsync(viewModel);

        return RedirectToAction(nameof(Details), new { id = result.Id });
    }

    /// <summary>
    /// Displays a confirmation page before changing the status of an advert.
    /// </summary>
    /// <param name="id">The ID of the advert whose status will be changed.</param>
    /// <param name="status">The new status to be set for the advert.</param>
    /// <returns>View with advert details and confirmation prompt, or NotFound/Forbid if not allowed.</returns>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ConfirmStatusChange(int id, string status)
    {
        var advert = await _advertService.GetAdvertByIdAsync(id);

        if (advert == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null || advert.Owner.Id != user.Id)
        {
            return Forbid();
        }

        ViewBag.NewStatus = status;
        return View(advert);
    }
}
