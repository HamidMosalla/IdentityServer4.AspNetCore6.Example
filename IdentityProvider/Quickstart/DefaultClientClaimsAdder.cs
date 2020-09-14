using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Validation;

namespace IdentityProvider.Quickstart
{
    public class DefaultClientClaimsAdder : ICustomTokenRequestValidator
    {
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            context.Result.ValidatedRequest.Client.AlwaysSendClientClaims = true;
            context.Result.ValidatedRequest.ClientClaims.Add(new Claim("Employee", "Mosalla"));
    
            return Task.FromResult(0);
        }
    }
}
