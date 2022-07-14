using IdentityModel.Client;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Client.Models;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Project.Client.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {          
            return View();
        }

        public async Task<IActionResult> Home()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (!String.IsNullOrEmpty(disco.Error))
            {
                throw new Exception(disco.Error);
            }
            var response = await client.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = IdentityModel.OidcConstants.GrantTypes.ClientCredentials,
                ClientId = "weatherApi",
                ClientSecret = "Mati",
                Parameters =
                {
                    { "username", "alice" },
                    { "password", "Pass123$" },
                    { "scope", "weatherApi.read" }
                }
            });
            if (response.IsError)
            {
                throw new Exception(response.Error);
            }

            var token = response.AccessToken;

            var response2 = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = disco.IntrospectionEndpoint,
                ClientId = "weatherApi",
                ClientSecret = "Mati",

                Token = token
            });
            if (response2.IsError) throw new Exception(response2.Error);

            var isActive = response2.IsActive;
            var claims = response2.Claims;

            var accessToken = await HttpContext.GetTokenAsync(IdentityServerConstants.TokenTypes.AccessToken);

            var response3 = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = accessToken
            });

            if (response3.IsError) throw new Exception(response3.Error);

            var claims2 = response3.Claims;
            return View();
        }

        public async Task<IActionResult> Signout()
        {
            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            var accessToken = await HttpContext.GetTokenAsync(IdentityServerConstants.TokenTypes.AccessToken);
            var idToken = await HttpContext.GetTokenAsync(IdentityServerConstants.TokenTypes.IdentityToken);
            var refreshToken = await HttpContext.GetTokenAsync(IdentityServerConstants.PersistedGrantTypes.RefreshToken);

            var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}