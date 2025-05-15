using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Pawesome.Interfaces;
using Pawesome.Models.Entities;
using Pawesome.Models.ViewModels.Auth;

namespace Pawesome.Services;

/// <summary>
/// Service that handles authentication-related operations including user registration, login, logout, and external login.
/// Works with ASP.NET Identity to manage user credentials and authentication state.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="userManager">ASP.NET Identity UserManager for user operations.</param>
    /// <param name="signInManager">ASP.NET Identity SignInManager for authentication operations.</param>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="mapper">AutoMapper for object mapping between DTOs and entities.</param>
    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="model">The registration data provided by the user.</param>
    /// <returns>
    /// An IdentityResult indicating the success or failure of the registration process.
    /// If successful, the user is automatically signed in.
    /// </returns>
    public async Task<IdentityResult> RegisterUserAsync(RegisterViewModel model)
    {
        if (await _userRepository.EmailExistsAsync(model.Email))
        {
            return IdentityResult.Failed(new IdentityError { Description = "Cet email est déjà utilisé" });
        }

        var user = _mapper.Map<User>(model);

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);
        }

        return result;
    }

    /// <summary>
    /// Authenticates a user with the provided credentials and adds claims if necessary.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="rememberMe">Whether to persist the authentication cookie across browser sessions.</param>
    /// <returns>
    /// A SignInResult indicating the success or failure of the login attempt.
    /// </returns>
    public async Task<SignInResult> LoginUserAsync(string email, string password, bool rememberMe)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return SignInResult.Failed;
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, false);

        if (!result.Succeeded) return result;
        
        var claims = new List<Claim>
        {
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("Id", user.Id.ToString())
        };
        
        if (user.Email != null) claims.Add(new Claim("Email", user.Email));
        
        if (!string.IsNullOrEmpty(user.Photo))
        {
            claims.Add(new Claim("Photo", user.Photo));
        }
        
        if (claims.Count != 0)
        {
            await _userManager.AddClaimsAsync(user, claims);
        }

        return result;
    }

    /// <summary>
    /// Signs out the currently logged-in user.
    /// </summary>
    /// <returns>A task representing the asynchronous sign-out operation.</returns>
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    /// <summary>
    /// Signs in a user using external authentication (e.g., Google).
    /// Adds claims and persists the authentication.
    /// </summary>
    /// <param name="user">The user to sign in.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ExternalLoginAsync(User user)
    {
        await _signInManager.SignOutAsync();

        var claims = new List<Claim>
        {
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("Id", user.Id.ToString())
        };

        if (user.Email != null) claims.Add(new Claim("Email", user.Email));

        if (!string.IsNullOrEmpty(user.Photo))
        {
            claims.Add(new Claim("Photo", user.Photo));
        }

        if (claims.Count != 0)
        {
            await _userManager.AddClaimsAsync(user, claims);
        }

        await _signInManager.SignInAsync(user, isPersistent: true);
    }
}
