using Microsoft.AspNetCore.Identity;
using Pawesome.Models.Dtos.Auth;

namespace Pawesome.Interfaces;

public interface IAuthService
{
    Task<IdentityResult> RegisterUserAsync(RegisterDto registerDto);
    Task<SignInResult> LoginUserAsync(string email, string password, bool rememberMe);
    Task LogoutAsync();
}