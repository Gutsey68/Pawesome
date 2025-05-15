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

    /// <summary>
    /// Initializes a new instance of the PetController
    /// </summary>
    /// <param name="petService">The pet service for handling pet operations</param>
    /// <param name="userManager">The user manager for handling user operations</param>
    public PetController(IPetService petService, UserManager<User> userManager)
    {
        _petService = petService;
        _userManager = userManager;
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
        var updatePetViewModel = await _petService.GetPetForEditAsync(id);

        if (updatePetViewModel == null)
        {
            return NotFound();
        }

        var userId = int.Parse(_userManager.GetUserId(User)!);
    
        var pets = await _petService.GetUserPetsAsync(userId);
    
        if (pets.All(p => p.Id != id))
        {
            return Forbid();
        }

        await PopulateAnimalTypesDropdown(updatePetViewModel.AnimalTypeId);
    
        return View(updatePetViewModel);
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

        var userId = int.Parse(_userManager.GetUserId(User)!);
    
        var pets = await _petService.GetUserPetsAsync(userId);
    
        if (pets.All(p => p.Id != petViewModel.Id))
        {
            return Forbid();
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

        var userId = int.Parse(_userManager.GetUserId(User)!);
        
        var pets = await _petService.GetUserPetsAsync(userId);
        
        if (pets.All(p => p.Id != id))
        {
            return Forbid();
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
        var userId = int.Parse(_userManager.GetUserId(User)!);
        
        var pets = await _petService.GetUserPetsAsync(userId);
        
        if (pets.All(p => p.Id != id))
        {
            return Forbid();
        }

        await _petService.DeletePetAsync(id);
        
        return RedirectToAction(nameof(Index));
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