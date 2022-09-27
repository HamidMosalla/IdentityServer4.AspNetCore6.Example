using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /*
                More info: http://hamidmosalla.com/2017/10/19/policy-based-authorization-using-asp-net-core-2-and-json-web-token-jwt/
            */

            var requestWithoutPolicyResponse = await RequestWithClientCredentialsWithoutPolicy();
            var requestWithClientCredetials = await RequestWithClientCredentialsWithPolicy();

            Console.WriteLine($"{nameof(requestWithoutPolicyResponse)} : {requestWithoutPolicyResponse}");
            Console.WriteLine($"{nameof(requestWithClientCredetials)} : {requestWithClientCredetials}");

            Console.ReadLine();
        }

        private static async Task<string> GetAccessToken()
        {
            var httpClient = new HttpClient();

            var openIdConnectEndPoint = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5000");

            var accessToken = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = openIdConnectEndPoint.TokenEndpoint,
                ClientId = "oauthClient",
                ClientSecret = "SuperSecretPassword",
                Scope = "api1.read",
            });

            if (accessToken.IsError)
            {
                Console.WriteLine(accessToken.Error);
                return accessToken.Error;

            }

            Console.WriteLine(accessToken.Json);

            return accessToken.AccessToken;
        }

        public static async Task<string> RequestWithClientCredentialsWithoutPolicy()
        {
            using (var client = new HttpClient())
            {
                var accessToken = await GetAccessToken();

                client.SetBearerToken(accessToken);

                var response = await client.GetAsync("https://localhost:5001/api/resource-with-policy");

                if (!response.IsSuccessStatusCode)
                {
                    return response.StatusCode.ToString();
                }

                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
        }

        public static async Task<string> RequestWithClientCredentialsWithPolicy()
        {
            using (var client = new HttpClient())
            {
                var accessToken = await GetAccessToken();

                client.SetBearerToken(accessToken);

                var response = await client.GetAsync("https://localhost:5001/api/resource-without-policy");

                if (!response.IsSuccessStatusCode)
                {
                    return response.StatusCode.ToString();
                }

                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
        }
    }
}