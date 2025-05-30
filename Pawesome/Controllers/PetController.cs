using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.Pet;

namespace Pawesome.Controllers;

/// <summary>
/// Controller handling all pet-related operations
/// </summary>
[Authorize]
public class PetController : Controller
{
    private readonly IPetService _petService;
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the PetController
    /// </summary>
    /// <param name="petService">The pet service for handling pet operations</param>
    /// <param name="userManager">The user manager for handling user operations</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    public PetController(IPetService petService, 
            UserManager<User> userManager,
            IMapper mapper)
    {
        _petService = petService;
        _userManager = userManager;
        _mapper = mapper;
    }
    
    /// <summary>
    /// Displays detailed information about a specific pet
    /// </summary>
    /// <param name="id">The ID of the pet</param>
    /// <returns>A view with the pet's details or NotFound if the pet doesn't exist</returns>
    public async Task<IActionResult> Details(int id)
    {
        var pet = await _petService.GetPetDetailsAsync(id);
    
        if (pet == null)
        {
            return NotFound();
        }
    
        var owner = await _userManager.FindByIdAsync(pet.UserId.ToString());
        var isAdmin = User.IsInRole("Admin");
    
        if (owner?.Status == Models.Enums.UserStatus.Banned && !isAdmin)
        {
            TempData["ErrorMessage"] = "Cet animal n'est pas accessible.";
            return RedirectToAction("Index", "Home");
        }
    
        return View(pet);
    }
    
    /// <summary>
    /// Displays the pet creation form
    /// </summary>
    /// <returns>The creation pet view with animal types dropdown</returns>
    public async Task<IActionResult> Create()
    {
        await PopulateAnimalTypesDropdown();
        
        return View();
    }
    
    /// <summary>
    /// Handles the pet creation form submission
    /// </summary>
    /// <param name="model">The pet creation model containing the pet data</param>
    /// <returns>Redirects to details on success or returns the form with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePetViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAnimalTypesDropdown(model.AnimalTypeId);
            return View(model);
        }

        var userId = int.Parse(_userManager.GetUserId(User)!);
    
        var petId = await _petService.CreatePetAsync(model, userId);
    
        return RedirectToAction(nameof(Details), new { id = petId });
    }
    /// <summary>
    /// Displays the pet edit form
    /// </summary>
    /// <param name="id">The ID of the pet to edit</param>
    /// <returns>The edit view, NotFound if pet doesn't exist, or Forbid if user doesn't own the pet</returns>
    public async Task<IActionResult> Edit(int id)
    {
        var pet = await _petService.GetPetForEditAsync(id);
    
        if (pet == null)
        {
            return NotFound();
        }
    
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
        var isAdmin = User.IsInRole("Admin");
    
        if (pet.UserId != currentUserId && !isAdmin)
        {
            return Forbid();
        }
    
        var owner = await _userManager.FindByIdAsync(pet.UserId.ToString());
    
        if (owner?.Status == Models.Enums.UserStatus.Banned && !isAdmin)
        {
            TempData["ErrorMessage"] = "Vous ne pouvez pas modifier cet animal.";
            return RedirectToAction("Index", "Home");
        }
    
        await PopulateAnimalTypesDropdown(pet.AnimalTypeId);
    
        return View(_mapper.Map<UpdatePetViewModel>(pet));
    }
    
    /// <summary>
    /// Handles the pet edit form submission
    /// </summary>
    /// <param name="petViewModel">The updated pet data</param>
    /// <returns>Redirects to details on success, returns the form with errors, or Forbid if user doesn't own the pet</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdatePetViewModel petViewModel)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAnimalTypesDropdown(petViewModel.AnimalTypeId);
            return View(petViewModel);
        }
    
        var pet = await _petService.GetPetForEditAsync(petViewModel.Id);
    
        if (pet == null)
        {
            return NotFound();
        }
    
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
        var isAdmin = User.IsInRole("Admin");
    
        if (pet.UserId != currentUserId && !isAdmin)
        {
            return Forbid();
        }
    
        var owner = await _userManager.FindByIdAsync(pet.UserId.ToString());
    
        if (owner?.Status == Models.Enums.UserStatus.Banned && !isAdmin)
        {
            TempData["ErrorMessage"] = "Vous ne pouvez pas modifier cet animal.";
            return RedirectToAction("Index", "Home");
        }
    
        await _petService.UpdatePetAsync(petViewModel);
    
        return RedirectToAction(nameof(Details), new { id = petViewModel.Id });
    }
    
    /// <summary>
    /// Displays the pet deletion confirmation page
    /// </summary>
    /// <param name="id">The ID of the pet to delete</param>
    /// <returns>The delete confirmation view, NotFound if pet doesn't exist, or Forbid if user doesn't own the pet</returns>
    public async Task<IActionResult> Delete(int id)
    {
        var pet = await _petService.GetPetDetailsAsync(id);
    
        if (pet == null)
        {
            return NotFound();
        }
    
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
        var isAdmin = User.IsInRole("Admin");
    
        if (pet.UserId != currentUserId && !isAdmin)
        {
            return Forbid();
        }
    
        var owner = await _userManager.FindByIdAsync(pet.UserId.ToString());
    
        if (owner?.Status == Models.Enums.UserStatus.Banned && !isAdmin)
        {
            TempData["ErrorMessage"] = "Vous ne pouvez pas supprimer cet animal.";
            return RedirectToAction("Index", "Home");
        }
    
        return View(pet);
    }

    /// <summary>
    /// Handles the pet deletion confirmation
    /// </summary>
    /// <param name="id">The ID of the pet to delete</param>
    /// <returns>Redirects to index on success or Forbid if user doesn't own the pet</returns>
    [HttpPost, ActionName("DeleteConfirmed")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pet = await _petService.GetPetDetailsAsync(id);
    
        if (pet == null)
        {
            return NotFound();
        }
    
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
        var isAdmin = User.IsInRole("Admin");
    
        if (pet.UserId != currentUserId && !isAdmin)
        {
            return Forbid();
        }
    
        var owner = await _userManager.FindByIdAsync(pet.UserId.ToString());
    
        if (owner?.Status == Models.Enums.UserStatus.Banned && !isAdmin)
        {
            TempData["ErrorMessage"] = "Vous ne pouvez pas supprimer cet animal.";
            return RedirectToAction("Index", "Home");
        }
    
        await _petService.DeletePetAsync(id);
    
        return RedirectToAction(nameof(Index), "User");
    }

    /// <summary>
    /// Populates the ViewBag with animal types for the dropdown list
    /// </summary>
    /// <param name="selectedAnimalTypeId">Optional: The ID of the currently selected animal type</param>
    private async Task PopulateAnimalTypesDropdown(int? selectedAnimalTypeId = null)
    {
        var animalTypes = await _petService.GetAnimalTypesAsync();
        
        ViewBag.AnimalTypes = new SelectList(animalTypes, "Id", "Name", selectedAnimalTypeId);
    }
}