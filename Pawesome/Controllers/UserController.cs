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
    /// Displays the public profile of another user
    /// </summary>
    /// <param name="id">The ID of the user whose profile to display</param>
    /// <returns>The public profile view if a user is found, NotFound result otherwise</returns>
    public async Task<IActionResult> Profile(int id)
    {
        var currentUserId = int.Parse(_userManager.GetUserId(User)!);
    
        if (id == currentUserId)
        {
            return RedirectToAction(nameof(Index));
        }
    
        var profileViewModel = await _userService.GetPublicUserProfileAsync(id, currentUserId);
    
        if (profileViewModel == null)
        {
            return NotFound();
        }
    
        return View(profileViewModel);
    }
}