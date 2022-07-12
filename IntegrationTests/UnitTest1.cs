using Microsoft.AspNetCore.Mvc.Testing;
using Project.IdentityServer;
using System.Net;

namespace IntegrationTests
{
    public class UnitTest1
    {
        private readonly HttpClient _client;

        public UnitTest1()
        {
            var appFactory = new WebApplicationFactory<Startup>();
            _client = appFactory.CreateClient();
        }
        [Fact]
        public async Task Test1()
        {
            var result = await _client.PostAsync("https://localhost:5001/connect/token", new StringContent("someVal"));
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}