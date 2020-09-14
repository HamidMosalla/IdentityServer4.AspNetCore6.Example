using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //[Authorize]
        [Authorize("Founder")]
        public IActionResult Privacy() => View();

        public async Task<IActionResult> CallApiUsingUserAccessToken()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(accessToken);

            // employee claim exists
            //var claim = await GetClaimsAsync(accessToken);

            // ApiResource
            var content = await client.GetStringAsync("https://localhost:5001/api/resource-with-policy");

            return View(model: content);
        }

        public async Task<Claim> GetClaimsAsync(string accessToken)
        {
            var client = new HttpClient();
            var metaDataResponse = await client.GetDiscoveryDocumentAsync("https://localhost:5000");

            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = metaDataResponse.UserInfoEndpoint,
                Token = accessToken
            });
            if (response.IsError)
            {
                throw new Exception("Problem while fetching data from the UserInfo endpoint", response.Exception);
            }
            var emailClaim = response.Claims.FirstOrDefault(c => c.Type.Equals("email"));
            var employeeNameClaim = response.Claims.FirstOrDefault(c => c.Type.Equals("Employee"));
            var addressClaim = response.Claims.FirstOrDefault(c => c.Type.Equals("address"));
            //User.AddIdentity(new ClaimsIdentity(new List<Claim> { new Claim(addressClaim.Type.ToString(), addressClaim.Value.ToString()) }));
            return addressClaim;
        }

        public async Task<IActionResult> CallApiUsingClientCredentials()
        {
            // AspNetCoreIdentityServer
            var httpClient = new HttpClient();

            var openIdConnectEndPoint = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5000");

            var accessToken = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = openIdConnectEndPoint.TokenEndpoint,
                ClientId = "oauthClient",
                ClientSecret = "SuperSecretPassword",
                Scope = "api1.read",
            });

            var client = new HttpClient();
            client.SetBearerToken(accessToken.AccessToken);
            // ApiResource
            var content = await client.GetStringAsync("https://localhost:5001/api/resource-without-policy");

            return View(content);
        }
    }
}
