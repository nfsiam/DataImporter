using DataImporter.Membership.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataImporter.Membership.Services
{
    public class ApplicationUserClaimsPrincipalFactory 
        : UserClaimsPrincipalFactory<ApplicationUser, Role>
    {
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<Role> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        { }
        public async override Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);
            var identity = (ClaimsIdentity)principal.Identity;

            var claims = new List<Claim>();
            if (user.Name != null)
            {
                claims.Add(new Claim("DisplayName", user.Name));
            }

            identity?.AddClaims(claims);
            return principal;
        }
	}
}
