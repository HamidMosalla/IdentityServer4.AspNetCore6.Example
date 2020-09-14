using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Quickstart
{
    public class ProfileService : IProfileService
    {
        // Note to self: for some reason the profile service does not gets called, I think it's the chosen flow (ie code)
        // For now I used ICustomTokenRequestValidator to manually add claims to the generated token
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.IssuedClaims.AddRange(context.Subject.Claims);

            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(0);
        }
    }
}