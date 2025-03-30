using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models.DTOs;

namespace Pawesome.Controllers;

/// <summary>
/// Controller managing user authentication and registration.
/// Provides functionalities for account creation, login, and logout.
/// </summary>
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterDto> _registerValidator;

    /// <summary>
    /// Initializes a new instance of the authentication controller.
    /// </summary>
    /// <param name="authService">Authentication service to manage user login and registration.</param>
    /// <param name="registerValidator">Validator for registration data.</param>
    public AuthController(IAuthService authService, IValidator<RegisterDto> registerValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
    }

    /// <summary>
    /// Displays the registration form.
    /// </summary>
    /// <returns>The registration form view.</returns>
    [HttpGet]
    public IActionResult Register() => View();

    /// <summary>
    /// Processes a new user registration request.
    /// </summary>
    /// <param name="model">Registration data provided by the user.</param>
    /// <returns>
    /// Redirects to the home page if registration is successful,
    /// or returns the registration view with validation errors if it fails.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        var validationResult = await _registerValidator.ValidateAsync(model);
        
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(model);
        }

        var result = await _authService.RegisterUserAsync(model);
        
        if (result.Succeeded)
            return RedirectToAction("Index", "Home");
        
        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
        
        return View(model);
    }

    /// <summary>
    /// Displays the login form.
    /// </summary>
    /// <returns>The login form view.</returns>
    [HttpGet]
    public IActionResult Login() => View();

    /// <summary>
    /// Processes a user login request.
    /// </summary>
    /// <param name="model">Login information provided by the user.</param>
    /// <returns>
    /// Redirects to the home page if login is successful,
    /// or returns the login view with an error message if it fails.
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginUserAsync(model.Email, model.Password, model.RememberMe);
        
        if (result.Succeeded)
            return RedirectToAction("Index", "Home");
        
        ModelState.AddModelError(string.Empty, "Identifiant ou mot de passe incorrect");
        return View(model);
    }

    /// <summary>
    /// Logs out the currently logged-in user.
    /// </summary>
    /// <returns>Redirects the user to the home page after logout.</returns>
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}