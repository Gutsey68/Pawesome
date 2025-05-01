using Microsoft.AspNetCore.Identity;
using Pawesome.Models;
using Pawesome.Models.ViewModels.Auth;

namespace Pawesome.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterUserAsync(RegisterViewModel registerDto);
    Task<SignInResult> LoginUserAsync(string email, string password, bool rememberMe);
    Task LogoutAsync();
    Task ExternalLoginAsync(User user);
}