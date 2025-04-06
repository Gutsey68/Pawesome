using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models;

namespace Pawesome.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly UserManager<User> _userManager;

    public ProfileController(IProfileService profileService, UserManager<User> userManager)
    {
        _profileService = profileService;
        _userManager = userManager;
    }

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