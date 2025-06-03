using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Pawesome.Models.Entities;
using System.Security.Claims;

namespace Pawesome.Infrastructure.Security
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole<int>>
    {
        public CustomClaimsPrincipalFactory(
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        { }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            identity.AddClaim(new Claim("FirstName", user.FirstName ?? ""));
            identity.AddClaim(new Claim("LastName", user.LastName ?? ""));
            identity.AddClaim(new Claim("Id", user.Id.ToString()));
            identity.AddClaim(new Claim("Email", user.Email ?? ""));
            identity.AddClaim(new Claim("Photo", user.Photo ?? ""));
            identity.AddClaim(new Claim("Address", user.Address?.StreetAddress ?? ""));

            return identity;
        }
    }
}