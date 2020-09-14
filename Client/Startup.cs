using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Client
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "cookie";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("cookie")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "cookie";
                    options.Authority = "https://localhost:5000";
                    options.RequireHttpsMetadata = true;
                    options.ClientId = "oidcClient";
                    options.ClientSecret = "SuperSecretPassword";
                    options.ResponseType = "code";
                    options.UsePkce = true;
                    options.ResponseMode = "query";

                    // Tried and failed
                    //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                    //options.ClaimActions.MapAll();
                    //options.GetClaimsFromUserInfoEndpoint = true;
                    //options.ClaimActions.MapUniqueJsonKey("sub", "sub");
                    //options.ClaimActions.MapUniqueJsonKey("name", "name");
                    //options.ClaimActions.MapUniqueJsonKey("given_name", "given_name");
                    //options.ClaimActions.MapUniqueJsonKey("family_name", "family_name");
                    ////options.ClaimActions.MapUniqueJsonKey("profile", "profile");
                    //options.ClaimActions.MapUniqueJsonKey("email", "email");
                    options.ClaimActions.MapUniqueJsonKey("Employee", "Employee");
                    options.ClaimActions.MapUniqueJsonKey("address", "address");

                     options.CallbackPath = "/signin-oidc"; // default redirect URI

                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         RoleClaimType = "Employee"
                     };

                    // options.Scope.Add("oidc"); // default scope
                    // options.Scope.Add("profile"); // default scope

                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;

                    options.Scope.Add("api1.read");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("email");
                    options.Scope.Add("Employee");
                    options.Scope.Add("address");

                });

            services.AddAuthorization(options => options.AddPolicy("Founder", policy => policy.RequireClaim("client_Employee", "Mosalla")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
