using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Advert;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.Advert;

namespace Pawesome.Controllers;

/// <summary>
/// Controller for managing pet sitting offers and requests
/// </summary>
public class AdvertController : Controller
{
    private readonly IAdvertService _advertService;
    private readonly IPetService _petService;
    private readonly IAnimalTypeService _animalTypeService;
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Initializes a new instance of the AdvertController
    /// </summary>
    /// <param name="advertService">Service for pet sitting operations</param>
    /// <param name="petService">Service for pet operations</param>
    /// <param name="animalTypeService">Service for animal type operations</param>
    /// <param name="userManager">Identity user manager</param>
    public AdvertController(
        IAdvertService advertService,
        IPetService petService,
        IAnimalTypeService animalTypeService,
        UserManager<User> userManager)
    {
        _advertService = advertService;
        _petService = petService;
        _animalTypeService = animalTypeService;
        _userManager = userManager;
    }

    /// <summary>
    /// Displays a list of all pet-sitting adverts
    /// </summary>
    /// <param name="isPetSitter">If true, shows pet sitting offers; if false, shows pet sitting requests</param>
    /// <returns>View containing a list of adverts</returns>
    [HttpGet]
    // public async Task<IActionResult> Index(
    //     bool isPetSitter = false,
    //     [FromQuery] SortingOptions? sortOptions = null)
    // {
    //     // Récupération de la liste des adverts en totalitée.
    //     var adverts = await _advertService.GetAllAdvertsAsync(isPetSitter);
    //
    //     // Ajout de la valeur de isPetSitter dans le RouteData
    //     RouteData.Values["isPetSitter"] = isPetSitter.ToString().ToLower();
    //     
    //     //=========== Gestion Prix ==============
    //     #region Gestion Prix
    //     
    //     //Récupération data GET
    //     var viewMinPrice = (object?)Request.Query["minPrice"].FirstOrDefault();
    //     var viewMaxPrice = (object?)Request.Query["maxPrice"].FirstOrDefault();
    //     // Avoir sur l'ensemble des annonce la valeur la plus petite et la plus grande niveau prix
    //     var minPrice = adverts.Min(a => a.Amount);
    //     var maxPrice = adverts.Max(a => a.Amount);
    //     
    //     ViewData["MinPrice"] = viewMinPrice ?? (int)Math.Round(minPrice);
    //     ViewData["MaxPrice"] = viewMaxPrice ?? (int)Math.Round(maxPrice);
    //     
    //     ViewData["MaxPriceBeforeReload"] = (int)Math.Round(maxPrice);
    //     ViewData["MinPriceBeforeReload"] = (int)Math.Round(minPrice);
    //     #endregion
    //     
    //     //=========== Gestion Type Animal ==============
    //     #region Gestion Popularité
    //     
    //     //Récupération de la liste des race en fonction des annonces
    //     var animalType = adverts.SelectMany(a => a.PetCartViewModels)
    //         .Select(p => p.Species)
    //         .Distinct()
    //         .ToList();
    //     ViewData["AnimalTypes"] = animalType.Select(name => new AnimalTypeDto { Name = name }).ToList();
    //     
    //     var selectedAnimalTypes = Request.Query["animalType"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
    //     ViewData["AnimalTypeInput"] = string.Join(",", selectedAnimalTypes);
    //     
    //     foreach (var type in selectedAnimalTypes)
    //     {
    //         ViewData[$"AnimalType-{type.Replace(" ", "")}"] = selectedAnimalTypes.Contains(type, StringComparer.OrdinalIgnoreCase) ? "checked" : "";
    //     }
    //     #endregion
    //     
    //     //=========== Gestion Popularité ==============
    //     #region Gestion Popularité
    //     
    //     //Récupération data GET
    //     var viewMostViewed = Convert.ToBoolean(Request.Query["mostViewed"].FirstOrDefault());
    //     var viewMostContracted = Convert.ToBoolean(Request.Query["mostContracted"].FirstOrDefault());
    //     var viewBestRated = Convert.ToBoolean(Request.Query["bestRated"].FirstOrDefault());
    //     
    //     ViewData["MostViewed"] = viewMostViewed ? "checked" : "";
    //     ViewData["MostContracted"] = viewMostContracted ? "checked" : "";
    //     ViewData["BestRated"] = viewBestRated ? "checked" : "";
    //     #endregion
    //     
    //     // ========== Gestion Type Animal ==============
    //     
    //     
    //     return View(adverts);
    // }
    public async Task<IActionResult> Index(bool isPetSitter, [FromQuery] AdvertViewModel model)
    {
        var adverts = await _advertService.GetFilteredAdvertsAsync(model.IsPetSitter, model);
        
        RouteData.Values["isPetSitter"] = isPetSitter.ToString().ToLower();
        
        return View(adverts);
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

    /// <summary>
    /// Processes the submission of a pet sitting request
    /// </summary>
    /// <param name="dto">Data for the pet sitting request</param>
    /// <returns>Redirects to Details if successful, or returns the form with validation errors</returns>
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
        
            var pets = await _petService.GetUserPets(user.Id);
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
        var animalTypes = await _animalTypeService.GetAllAnimalTypesAsync();
        ViewBag.AnimalTypes = animalTypes;

        return View(new PetSittingOfferViewModel());
    }

    /// <summary>
    /// Processes the submission of a pet sitting offer
    /// </summary>
    /// <param name="dto">Data for the pet sitting offer</param>
    /// <returns>Redirects to Details if successful, or returns the form with validation errors</returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateOffer(PetSittingOfferViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var animalTypes = await _animalTypeService.GetAllAnimalTypesAsync();
            ViewBag.AnimalTypes = animalTypes;
        
            return View(viewModel);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
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
    public async Task<IActionResult> UpdateStatus(int advertId, string status)
    {
        var result = await _advertService.UpdateAdvertStatusAsync(advertId, status);

        if (!result)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Details), new { id = advertId });
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
        {
            return Challenge();
        }
        
        var adverts = await _advertService.GetUserAdvertsAsync(user.Id);
        
        Console.WriteLine("adverts", adverts);
        
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
        
        if (advert.Owner == null || advert.Owner.Id != user.Id)
        {
            return Forbid();
        }
        
        var updateModel = new UpdatePetSittingRequestViewModel
        {
            Id = advert.Id,
            StartDate = advert.StartDate,
            EndDate = advert.EndDate,
            Amount = advert.Amount,
            PetIds = advert.PetCartViewModels.Select(p => p.Id).ToList(),
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
        
        if (advert.Owner == null || advert.Owner.Id != user.Id)
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
            if (advert.PetCartViewModels.Any(p => p.Name == animalType.Name))
            {
                updateModel.AcceptedAnimalTypeIds.Add(animalType.Id);
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
    /// Displays the confirmation page for deleting an advert
    /// </summary>
    /// <param name="id">The ID of the advert to delete</param>
    /// <returns>View containing the delete confirmation form</returns>
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
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
    
        if (advert.Owner == null || advert.Owner.Id != user.Id)
        {
            return Forbid();
        }
    
        var viewModel = new DeleteAdvertViewModel
        {
            Id = advert.Id,
            Title = advert.IsPetSitter ? "Offre de pet sitting" : "Demande de pet sitting",
            IsPetSitter = advert.IsPetSitter,
            StartDate = advert.StartDate,
            EndDate = advert.EndDate
        };
    
        return View(viewModel);
    }

    /// <summary>
    /// Processes the deletion of an advert
    /// </summary>
    /// <param name="id">The ID of the advert to delete</param>
    /// <returns>Redirects to MyAdverts if successful, or returns NotFound/Forbid as appropriate</returns>
    [Authorize]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
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
    
        if (advert.Owner == null || advert.Owner.Id != user.Id)
        {
            return Forbid();
        }
    
        var result = await _advertService.DeleteAdvertAsync(id);
    
        if (!result)
        {
            return NotFound();
        }
    
        return RedirectToAction(nameof(MyAdverts));
    }
}