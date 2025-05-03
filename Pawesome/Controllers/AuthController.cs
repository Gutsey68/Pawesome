using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.Auth;

namespace Pawesome.Controllers;

/// <summary>
/// Controller handling user authentication and registration
/// </summary>
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;

    /// <summary>
    /// Initializes a new instance of the AuthController
    /// </summary>
    /// <param name="authService">The authentication service for handling login and registration</param>
    /// <param name="emailService">The email service for sending notifications</param>
    /// <param name="userManager">The user manager for user operations</param>
    public AuthController(
        IAuthService authService,
        IEmailService emailService,
        UserManager<User> userManager)
    {
        _authService = authService;
        _emailService = emailService;
        _userManager = userManager;
    }

    /// <summary>
    /// Displays the registration form
    /// </summary>
    /// <returns>The registration view</returns>
    [HttpGet]
    public IActionResult Register() => View();

    /// <summary>
    /// Handles the registration form submission
    /// </summary>
    /// <param name="model">The registration data</param>
    /// <param name="credential">The credential token provided by Google OAuth</param>
    /// <returns>Redirects to RegisterConfirmation on success, or returns the form with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? credential = null)
    {
        if (!string.IsNullOrEmpty(credential))
        {
            return await HandleGoogleAuthentication(credential, model);
        }
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _authService.RegisterUserAsync(model);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) return RedirectToAction("Index", "Home");

            await SendEmailConfirmation(user);

            return RedirectToAction("RegisterConfirmation");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    /// <summary>
    /// Sends a confirmation email to the user
    /// </summary>
    /// <param name="user">The user to send the confirmation email to</param>
    /// <returns>A task representing the asynchronous operation</returns>
    private async Task SendEmailConfirmation(User user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var callbackUrl = Url.Action("ConfirmEmail", "Auth",
            new { userId = user.Id, token = token }, protocol: Request.Scheme);

        if (user.Email != null)
        {
            await _emailService.SendEmailAsync(
                user.Email,
                "Confirmez votre compte Pawesome",
                $"<h1>Bienvenue sur Pawesome !</h1>" +
                $"<p>Merci pour votre inscription. Veuillez confirmer votre compte en " +
                $"<a href='{callbackUrl}'>cliquant ici</a>.</p>" +
                $"<p>Si vous n'avez pas créé de compte sur Pawesome, veuillez ignorer cet email.</p>");
        }
    }

    /// <summary>
    /// Displays the registration confirmation page
    /// </summary>
    /// <returns>The registration confirmation view</returns>
    [HttpGet]
    public IActionResult RegisterConfirmation() => View();

    /// <summary>
    /// Confirms a user's email address
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="token">The confirmation token</param>
    /// <returns>Confirmation view on success, error view on failure, or redirects home if parameters are invalid</returns>
    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound($"Unable to find user with ID '{userId}'.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        
        if (!result.Succeeded)
        {
            return NotFound();
        }

        return View(result.Succeeded ? "ConfirmEmail" : "Error");
    }

    /// <summary>
    /// Displays the login form
    /// </summary>
    /// <returns>The login view</returns>
    [HttpGet]
    public IActionResult Login() => View();

    /// <summary>
    /// Handles the login form submission and Google OAuth authentication
    /// </summary>
    /// <param name="model">The login credentials for standard login</param>
    /// <param name="credential">The credential token provided by Google OAuth</param>
    /// <returns>Redirects to the home page on successful login or returns the form with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? credential = null)
    {
        if (!string.IsNullOrEmpty(credential))
        {
            return await HandleGoogleAuthentication(credential, model);
        }
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var loginResult = await _authService.LoginUserAsync(model.Email, model.Password, model.RememberMe);

        if (loginResult.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "Email ou mot de passe incorrect.");
        return View(model);
    }

    /// <summary>
    /// Handles Google OAuth authentication for both login and registration
    /// </summary>
    /// <param name="credential">The credential token provided by Google OAuth</param>
    /// <param name="viewModel">The view model to return in case of error</param>
    /// <returns>Redirects to the home page on successful authentication or returns the form with errors</returns>
    private async Task<IActionResult> HandleGoogleAuthentication(string credential, object viewModel)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = ["293214798250-hhaj22tlc17aug4ojdmcn81li2sgkfi5.apps.googleusercontent.com"]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);
            
            var user = await _userManager.FindByEmailAsync(payload.Email);
            
            if (user == null)
            {
                var nameParts = payload.Name.Split(' ', 2);
                var firstName = nameParts.FirstOrDefault() ?? string.Empty;
                var lastName = nameParts.Length > 1 ? nameParts[1] : string.Empty;
                
                user = new User
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    EmailConfirmed = true,
                    FirstName = firstName,
                    LastName = lastName,
                    CreatedAt = DateTime.UtcNow,
                    Address = new Address
                    {
                        StreetAddress = "",
                        City = new City
                        {
                            Name = "",
                            PostalCode = "",
                            Country = new Country
                            {
                                Name = "",
                                Cities = new List<City>(),
                            },
                            Addresses = new List<Address>(),
                        },
                        Users = new List<User>(),
                    },
                    Pets = new List<Pet>(),
                    Notifications = new List<Notification>(),
                    Reports = new List<Report>(),
                    PasswordResets = new List<PasswordReset>(),
                    SentMessages = new List<Message>(),
                    ReceivedMessages = new List<Message>(),
                    Reviews = new List<Review>(),
                    Payments = new List<Payment>()
                };
                
                var result = await _userManager.CreateAsync(user);
                
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Erreur lors de la création du compte avec Google.");
                    return View(viewModel);
                }
            }

            await _authService.ExternalLoginAsync(user);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Erreur lors de l'authentification Google : {ex.Message}");
            return View(viewModel);
        }
    }

    /// <summary>
    /// Logs out the currently logged-in user
    /// </summary>
    /// <returns>Redirects to the home page</returns>
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();

        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Displays the forgot password form
    /// </summary>
    /// <returns>The forgot password view</returns>
    [HttpGet]
    public IActionResult ForgotPassword() => View();

    /// <summary>
    /// Handles the forgot password form submission
    /// </summary>
    /// <param name="model">The email address to send password reset instructions to</param>
    /// <returns>Redirects to the confirmation page in all cases for security</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var callbackUrl = Url.Action("ResetPassword", "Auth",
            new { email = user.Email, code = token }, protocol: Request.Scheme);

        await _emailService.SendEmailAsync(
            model.Email,
            "Réinitialisation de mot de passe",
            $"<h1>Réinitialisez votre mot de passe</h1>" +
            $"<p>Veuillez réinitialiser votre mot de passe en <a href='{callbackUrl}'>cliquant ici</a>.</p>" +
            $"<p>Si vous n'avez pas demandé de réinitialisation de mot de passe, veuillez ignorer cet email.</p>");

        return RedirectToAction("ForgotPasswordConfirmation");
    }

    /// <summary>
    /// Displays the forgot password confirmation page
    /// </summary>
    /// <returns>The forgot password confirmation view</returns>
    [HttpGet]
    public IActionResult ForgotPasswordConfirmation() => View();

    /// <summary>
    /// Displays the reset password form
    /// </summary>
    /// <param name="email">The email address of the user</param>
    /// <param name="code">The password reset token</param>
    /// <returns>The reset password view or redirects home if parameters are invalid</returns>
    [HttpGet]
    public IActionResult ResetPassword(string email, string code)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
        {
            return RedirectToAction("Index", "Home");
        }

        var model = new ResetPasswordViewModel
        {
            Email = email,
            Code = code,
            Password = string.Empty,
            ConfirmPassword = string.Empty
        };

        return View(model);
    }

    /// <summary>
    /// Handles the reset password form submission
    /// </summary>
    /// <param name="model">The reset password data including the new password</param>
    /// <returns>Redirects to the confirmation page on success or returns the form with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return RedirectToAction("ResetPasswordConfirmation");
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

        if (result.Succeeded)
        {
            return RedirectToAction("ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    /// <summary>
    /// Displays the reset password confirmation page
    /// </summary>
    /// <returns>The reset password confirmation view</returns>
    [HttpGet]
    public IActionResult ResetPasswordConfirmation() => View();
}