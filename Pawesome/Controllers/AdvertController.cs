using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.DTOs;
using Pawesome.Models.Dtos.Advert;

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
    public async Task<IActionResult> Index(bool isPetSitter = false)
    {
        var adverts = await _advertService.GetAllAdvertsAsync(isPetSitter);
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
        
        return View(new PetSittingRequestDto());
    }

    /// <summary>
    /// Processes the submission of a pet sitting request
    /// </summary>
    /// <param name="dto">Data for the pet sitting request</param>
    /// <returns>Redirects to Details if successful, or returns the form with validation errors</returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateRequest(PetSittingRequestDto dto)
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
            
            return View(dto);
        }

        var existingUser = await _userManager.GetUserAsync(User);

        if (existingUser == null)
        {
            return Challenge();
        }
        
        var result = await _advertService.CreatePetSittingRequestAsync(dto, existingUser.Id);
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

        return View(new PetSittingOfferDto());
    }

    /// <summary>
    /// Processes the submission of a pet sitting offer
    /// </summary>
    /// <param name="dto">Data for the pet sitting offer</param>
    /// <returns>Redirects to Details if successful, or returns the form with validation errors</returns>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateOffer(PetSittingOfferDto dto)
    {
        if (!ModelState.IsValid)
        {
            var animalTypes = await _animalTypeService.GetAllAnimalTypesAsync();
            ViewBag.AnimalTypes = animalTypes;
            
            return View(dto);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }
        
        var result = await _advertService.CreatePetSittingOfferAsync(dto, user.Id);

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
        
        return View(adverts);
    }
}