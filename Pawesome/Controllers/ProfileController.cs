using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models;

namespace Pawesome.Controllers;

/// <summary>
/// Controller handling user profile operations
/// </summary>
[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Initializes a new instance of the ProfileController
    /// </summary>
    /// <param name="profileService">Service for handling profile operations</param>
    /// <param name="userManager">The user manager for handling user operations</param>
    public ProfileController(IProfileService profileService, UserManager<User> userManager)
    {
        _profileService = profileService;
        _userManager = userManager;
    }

    /// <summary>
    /// Displays the user's profile page
    /// </summary>
    /// <returns>The profile view if user is found, redirects to login otherwise</returns>
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
    
        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }
    
        var profileViewModel = await _profileService.GetUserProfileAsync(userId);

        if (profileViewModel == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        return View(profileViewModel);
    }
}