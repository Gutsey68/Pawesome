using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.User;

namespace Pawesome.Controllers;

/// <summary>
/// Controller handling user profile operations
/// </summary>
[Authorize]
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IBookingService _bookingService;

    /// <summary>
    /// Initializes a new instance of the UserController
    /// </summary>
    /// <param name="userService">Service for handling profile operations</param>
    /// <param name="userManager">The user manager for handling user operations</param>
    /// <param name="signInManager">The sign-in manager for handling user sign-in operations</param>
    /// <param name="bookingService">Service pour gérer les réservations</param>
    public UserController(
        IUserService userService, 
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IBookingService bookingService)
    {
        _userService = userService;
        _userManager = userManager;
        _signInManager = signInManager;
        _bookingService = bookingService;
    }

    /// <summary>
    /// Displays the user's profile page
    /// </summary>
    /// <returns>The profile view if user is found, redirects to log in otherwise</returns>
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var profileViewModel = await _userService.GetUserProfileAsync(int.Parse(userId));

        if (profileViewModel == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var pendingBookings = await _bookingService.GetPendingBookingsForUserAdvertsAsync(int.Parse(userId));
    
        profileViewModel.PendingBookings = pendingBookings;

        return View(profileViewModel);
    }

    /// <summary>
    /// Displays the edit form for a user profile
    /// </summary>
    /// <param name="id">The ID of the user to edit</param>
    /// <returns>The edit view if a user is found, NotFound result otherwise</returns>
    public async Task<IActionResult> Edit(int id)
    {
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
        var isAdmin = User.IsInRole("Admin");
    
        if (id != currentUserId && !isAdmin)
        {
            return Forbid();
        }
        var updateUserDto = await _userService.GetUserForEditAsync(id);

        if (updateUserDto == null)
        {
            return NotFound();
        }

        return View(updateUserDto);
    }

    /// <summary>
    /// Processes the user profile update form submission
    /// </summary>
    /// <param name="model">The model containing updated user information</param>
    /// <returns>Redirects to the profile page on success, login page if user not found</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateUserViewModel model)
    {
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
        var isAdmin = User.IsInRole("Admin");
    
        if (model.Id != currentUserId && !isAdmin)
        {
            return Forbid();
        }
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = int.Parse(_userManager.GetUserId(User)!);
        var user = await _userService.GetUserProfileAsync(userId);

        if (user == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        await _userService.UpdateUserAsync(model);
    
        await _signInManager.RefreshSignInAsync(await _userManager.FindByIdAsync(userId.ToString()) ?? throw new InvalidOperationException());
    
        return RedirectToAction(nameof(Index), new { id = model.Id });
    }
    
    /// <summary>
    /// Displays the public profile of a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user whose profile is to be displayed.</param>
    /// <returns>
    /// The public profile view if the user exists and is accessible; 
    /// redirects to the current user's profile or home page if not accessible; 
    /// returns NotFound if the user does not exist.
    /// </returns>
    public async Task<IActionResult> Profile(int id)
    {
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
        var isAdmin = User.IsInRole("Admin");

        if (id == currentUserId)
        {
            return RedirectToAction(nameof(Index));
        }

        var user = await _userManager.FindByIdAsync(id.ToString());
    
        if (user == null)
        {
            return NotFound();
        }
    
        if (user.Status == Models.Enums.UserStatus.Banned && !isAdmin)
        {
            TempData["ErrorMessage"] = "Ce profil n'est pas accessible.";
            return RedirectToAction("Index", "Home");
        }

        var profileViewModel = await _userService.GetPublicUserProfileAsync(id, currentUserId);

        if (profileViewModel == null)
        {
            return NotFound();
        }

        return View(profileViewModel);
    }
    
    /// <summary>
    /// Allows a user to rate another user for a specific advert.
    /// </summary>
    /// <param name="advertId">The ID of the advert related to the rating.</param>
    /// <param name="ratedUserId">The ID of the user being rated.</param>
    /// <param name="rating">The rating value (must be between 1 and 5).</param>
    /// <param name="comment">An optional comment for the rating.</param>
    /// <returns>
    /// Redirects to the advert details page with a success or error message depending on the outcome.
    /// </returns>
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RateUser(int advertId, int ratedUserId, int rating, string? comment)
    {
        if (rating < 1 || rating > 5)
        {
            TempData["ErrorMessage"] = "La note doit être comprise entre 1 et 5.";
            return RedirectToAction("Details", "Advert", new { id = advertId });
        }

        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
    
        if (currentUserId == ratedUserId)
        {
            TempData["ErrorMessage"] = "Vous ne pouvez pas vous évaluer vous-même.";
            return RedirectToAction("Details", "Advert", new { id = advertId });
        }

        if (advertId <= 0 || ratedUserId <= 0)
        {
            TempData["ErrorMessage"] = "Paramètres invalides pour l'évaluation.";
            return RedirectToAction("Details", "Advert", new { id = advertId });
        }

        try
        {
            var success = await _userService.RateUserAsync(currentUserId, ratedUserId, rating, comment, advertId);
        
            if (success)
            {
                TempData["SuccessMessage"] = "Votre évaluation a été enregistrée avec succès.";
            }
            else
            {
                TempData["ErrorMessage"] = "Une erreur est survenue lors de l'enregistrement de votre évaluation.";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Erreur: {ex.Message}";
        }
    
        return RedirectToAction("Details", "Advert", new { id = advertId });
    }
}
