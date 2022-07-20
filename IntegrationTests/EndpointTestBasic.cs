using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Moq;
using Project.API;
using Project.API.Controllers;

namespace IntegrationTests
{
    public class EndpointTestBasic : IClassFixture<WebApplicationFactory<Project.IdentityServer.Startup>>
    {
        private readonly WebApplicationFactory<Project.IdentityServer.Startup> _appFactory;
        private readonly HttpClient httpClient;

        public EndpointTestBasic(WebApplicationFactory<Project.IdentityServer.Startup> appFactory)
        {
            _appFactory = appFactory;
            httpClient = appFactory.CreateClient();
        }

        [Fact]
        public void GetWeather()
        {
            Mock<ILogger<WeatherForecastController>> loggerMock = new();
            var controller = new WeatherForecastController(loggerMock.Object);

            IEnumerable<WeatherForecast> result = controller.Get();

            Assert.Equal(5, result.Count());
        }

        [Fact]
        public async Task GetAccessTokenForUser()
        {
            string token = await GetAccessToken();

            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        private async Task<string> GetAccessToken()
        {
            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (!String.IsNullOrEmpty(disco.Error))
            {
                throw new Exception(disco.Error);
            }
            var response = await httpClient.RequestTokenAsync(new IdentityModel.Client.TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = IdentityModel.OidcConstants.GrantTypes.ClientCredentials,
                ClientId = "weatherApi",
                ClientSecret = "Mati",
                Parameters =
                {
                    { "scope", "weatherApi.read"}
                }
            });
            return response.AccessToken;
        }

    }
}