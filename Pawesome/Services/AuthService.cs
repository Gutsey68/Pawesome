using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Pawesome.Interfaces;
using Pawesome.Models;
using Pawesome.Models.DTOs;

namespace Pawesome.Services;

/// <summary>
/// Service that handles authentication-related operations including user registration, login, and logout.
/// Works with ASP.NET Identity to manage user credentials and authentication state.
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    /// <summary>
    /// Initializes a new instance of the authentication service.
    /// </summary>
    /// <param name="userManager">ASP.NET Identity UserManager for user operations.</param>
    /// <param name="signInManager">ASP.NET Identity SignInManager for authentication operations.</param>
    /// <param name="userRepository">Repository for user data access.</param>
    /// <param name="mapper">AutoMapper for object mapping between DTOs and entities.</param>
    /// <param name="roleManager">ASP.NET Identity RoleManager for role operations.</param>
    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IUserRepository userRepository,
        IMapper mapper,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userRepository = userRepository;
        _mapper = mapper;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="registerDto">The registration data provided by the user.</param>
    /// <returns>
    /// An IdentityResult indicating success or failure of the registration process.
    /// If successful, the user is automatically signed in.
    /// </returns>
    public async Task<IdentityResult> RegisterUserAsync(RegisterDto registerDto)
    {
        if (await _userRepository.EmailExistsAsync(registerDto.Email))
        {
            return IdentityResult.Failed(new IdentityError { Description = "Cet email est déjà utilisé" });
        }

        var user = _mapper.Map<User>(registerDto);
        
        var userRole = await _roleManager.FindByNameAsync("User");
        user.RoleId = Convert.ToInt32(userRole.Id);
    
        var result = await _userManager.CreateAsync(user, registerDto.Password);
    
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);
        }
    
        return result;
    }

    /// <summary>
    /// Authenticates a user with the provided credentials.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="rememberMe">Whether to persist the authentication cookie across browser sessions.</param>
    /// <returns>
    /// A SignInResult indicating success or failure of the login attempt.
    /// </returns>
    public async Task<SignInResult> LoginUserAsync(string email, string password, bool rememberMe)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return SignInResult.Failed;
        }

        return await _signInManager.PasswordSignInAsync(user, password, rememberMe, false);
    }

    /// <summary>
    /// Signs out the currently logged-in user.
    /// </summary>
    /// <returns>A task representing the asynchronous sign-out operation.</returns>
    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}