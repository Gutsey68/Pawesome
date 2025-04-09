using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.DTOs;

namespace Pawesome.Controllers;

/// <summary>
/// Controller handling user authentication and registration
/// </summary>
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<ResetPasswordDto> _resetPasswordValidator;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the AuthController
    /// </summary>
    /// <param name="authService">The authentication service for handling login and registration</param>
    /// <param name="emailService">The email service for sending notifications</param>
    /// <param name="userManager">The user manager for user operations</param>
    /// <param name="registerValidator">Validator for registration data</param>
    /// <param name="resetPasswordValidator">Validator for password reset data</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    /// <param name="logger">Logger for recording operations</param>
    public AuthController(
        IAuthService authService,
        IEmailService emailService,
        UserManager<User> userManager,
        IValidator<RegisterDto> registerValidator,
        IValidator<ResetPasswordDto> resetPasswordValidator,
        IMapper mapper,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _emailService = emailService;
        _userManager = userManager;
        _registerValidator = registerValidator;
        _resetPasswordValidator = resetPasswordValidator;
        _mapper = mapper;
        _logger = logger;
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
    /// <returns>Redirects to RegisterConfirmation on success, or returns the form with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
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
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            
            if (user == null) return RedirectToAction("Index", "Home");
            
            try
            {
                await SendEmailConfirmation(user);
                _logger.LogInformation("Confirmation email sent to {Email}", model.Email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending confirmation email to {Email}", model.Email);
            }
            return RedirectToAction("RegisterConfirmation");

        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

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
            await _emailService.SendEmailAsync(
                user.Email,
                "Confirm your Pawesome account",
                $"<h1>Welcome to Pawesome!</h1>" +
                $"<p>Thank you for registering. Please confirm your account by " +
                $"<a href='{callbackUrl}'>clicking here</a>.</p>" +
                $"<p>If you did not create an account on Pawesome, please ignore this email.</p>");
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
        if (result.Succeeded)
        {
            return View("ConfirmEmail");
        }

        return View("Error");
    }

    /// <summary>
    /// Displays the login form
    /// </summary>
    /// <returns>The login view</returns>
    [HttpGet]
    public IActionResult Login() => View();

    /// <summary>
    /// Handles the login form submission
    /// </summary>
    /// <param name="model">The login credentials</param>
    /// <returns>Redirects to home page on success, or returns the form with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginDto model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginUserAsync(model.Email, model.Password, model.RememberMe);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        ModelState.AddModelError(string.Empty, "Invalid login attempt");
        return View(model);
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
    /// <param name="validator">The validator for the forgot password form</param>
    /// <returns>Redirects to confirmation page in all cases for security</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model, [FromServices] IValidator<ForgotPasswordDto> validator)
    {
        _logger.LogInformation("Processing password reset request for: {Email}", model.Email);

        var validationResult = await validator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
        {
            _logger.LogWarning("Failed ForgotPasswordAsync for: {Email} - User non-existent or email not confirmed", model.Email);
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        try
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Auth",
                new { email = user.Email, code = token }, protocol: Request.Scheme);

            await _emailService.SendEmailAsync(
                model.Email,
                "Reset Password",
                $"<h1>Reset your password</h1>" +
                $"<p>Please reset your password by <a href='{callbackUrl}'>clicking here</a>.</p>" +
                $"<p>If you did not request a password reset, please ignore this email.</p>");

            _logger.LogInformation("Password reset email sent to {Email}", model.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset email to {Email}", model.Email);
        }

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

        var model = new ResetPasswordDto
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
    /// <returns>Redirects to confirmation page on success, or returns the form with errors</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
    {
        var validationResult = await _resetPasswordValidator.ValidateAsync(model);
        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState);
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
            _logger.LogInformation("Password successfully reset for {Email}", model.Email);
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

    /// <summary>
    /// Development route to manually confirm a user's email
    /// </summary>
    /// <param name="email">The email address to confirm</param>
    /// <returns>Text confirmation of the operation result</returns>
    [HttpGet]
    [Route("/dev-confirm-email")]
    public async Task<IActionResult> ConfirmEmailDev(string email)
    {
        if (string.IsNullOrEmpty(email))
            return Content("Email not specified");

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return Content("User not found");

        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);
        _logger.LogInformation("Email manually confirmed for {Email}", email);
        return Content($"Email confirmed for {email}");
    }

    /// <summary>
    /// Test route for email sending functionality
    /// </summary>
    /// <param name="email">Optional email address to send the test to</param>
    /// <returns>Text confirmation of the operation result</returns>
    [HttpGet]
    [Route("/test-email")]
    public async Task<IActionResult> TestEmail(string email = null!)
    {
        try
        {
            email ??= "test@example.com";

            await _emailService.SendEmailAsync(
                email,
                "Test email",
                "<h1>This is a test</h1><p>If you receive this email, the email service is working correctly.</p>");

            _logger.LogInformation("Test email sent to {Email}", email);
            return Content($"Test email sent to {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending test email to {Email}", email);
            return Content($"Error sending email: {ex.Message}");
        }
    }
}