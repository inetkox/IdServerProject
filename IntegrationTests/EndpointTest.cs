using System.Net;

namespace IntegrationTests
{
    public class EndpointTest : IClassFixture<AppInstance>
    {
        private readonly AppInstance _instance;

        public EndpointTest(AppInstance instance)
        {
            _instance = instance;
        }

        [Fact]
        public async Task WeatherEndpoint()
        {
            var client = _instance
                .AuthenticatedInstance()
                .CreateClient(new()
                {
                    AllowAutoRedirect = false,
                });

            var result = await client.GetAsync("https://localhost:5002/WeatherForecast");
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var content = await result.Content.ReadAsStringAsync();
            Assert.Contains("date", content);
        }
    }
}
