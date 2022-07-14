using Microsoft.AspNetCore.Mvc.Testing;
using Project.IdentityServer;
using System.Net;

namespace IntegrationTests
{
    public class UnitTest1
    {
        private readonly HttpClient httpClient;

        public UnitTest1()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            httpClient = appFactory.CreateClient();
        }
        [Fact]
        public async Task Test1()
        {
            var apiClient = new HttpClient();

            var apiResponse = await apiClient.GetAsync("https://localhost:5004/api/weather");

            Assert.True(apiResponse.IsSuccessStatusCode);

            var stringResponse = await apiResponse.Content.ReadAsStringAsync();

            Assert.Equal("Healthy", stringResponse);
        }
    }
}