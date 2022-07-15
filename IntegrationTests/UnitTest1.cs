using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Client.Controllers;
using Project.IdentityServer;
using System.Web;

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
        public async Task GetAccessTokenForUser()
        {
            string token = await GetAccessToken("alice", "Pass123$");

            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public async Task GetUserInfo()
        {
            string token = await GetAccessToken("alice", "Pass123$");

            httpClient.SetBearerToken(token);

            var apiResponse = await httpClient.GetAsync("https://localhost:5001/connect/userinfo");

            Assert.True(apiResponse.IsSuccessStatusCode);

            var stringResponse = await apiResponse.Content.ReadAsStringAsync();

            var result = (JObject?)JsonConvert.DeserializeObject(stringResponse);
            var value = result?.GetValue("preferred_username");
            Assert.Equal("alice", value);
        }

        [Fact]
        public async Task GetUserAuthorize()
        {
            string token = await GetAccessToken("alice", "Pass123$");

            httpClient.SetBearerToken(token);

            var introToken = await GetIntrospectToken();

            var revokeToken = await GetRevokeToken();

            var deviceToken = await GetDeviceToken();

            var idToken = await GetIdToken();

            var apiResponse = await httpClient.GetAsync("https://localhost:5001/connect/userinfo");

            Assert.True(apiResponse.IsSuccessStatusCode);

            var stringResponse = await apiResponse.Content.ReadAsStringAsync();

            var result = (JObject?)JsonConvert.DeserializeObject(stringResponse);
            var value = result?.GetValue("preferred_username");
            Assert.Equal("alice", value);
        }

        private async Task<string> GetAccessToken(string username, string password)
        {
            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (!String.IsNullOrEmpty(disco.Error))
            {
                throw new Exception(disco.Error);
            }
            var response = await httpClient.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = IdentityModel.OidcConstants.GrantTypes.ClientCredentials,
                ClientId = "weatherApi",
                ClientSecret = "Mati",
                Parameters =
                {
                    { "username", username},
                    { "password", password},
                    { "scope", "weatherApi.read"}
                }
            });
            return response.AccessToken;
        }

        private async Task<string> GetIdToken()
        {
            string accessToken = await GetAccessToken("alice", "Pass123$");
            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (!String.IsNullOrEmpty(disco.Error))
            {
                throw new Exception(disco.Error);
            }

            var response = await httpClient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = accessToken             
            });
            return response.Raw;
        }

        private async Task<string> GetIntrospectToken()
        {
            string accessToken = await GetAccessToken("alice", "Pass123$");
            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (!String.IsNullOrEmpty(disco.Error))
            {
                throw new Exception(disco.Error);
            }

            var response = await httpClient.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                Address = disco.IntrospectionEndpoint,
                ClientId = "weatherApi",
                ClientSecret = "Mati",
                Token = accessToken
            });
            if (response.IsError) throw new Exception(response.Error);

            var isActive = response.IsActive;
            var claims = response.Claims;
            return response.Raw;
        }

        private async Task<string> GetRevokeToken()
        {
            string accessToken = await GetAccessToken("alice", "Pass123$");
            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (!String.IsNullOrEmpty(disco.Error))
            {
                throw new Exception(disco.Error);
            }

            var response = await httpClient.RevokeTokenAsync(new TokenRevocationRequest
            {
                Address = disco.RevocationEndpoint,
                ClientId = "weatherApi",
                ClientSecret = "Mati",
                Token = accessToken
            });
            if (response.IsError) throw new Exception(response.Error);
            return response.Raw;
        }

        private async Task<string> GetDeviceToken()
        {
            string accessToken = await GetAccessToken("alice", "Pass123$");
            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (!String.IsNullOrEmpty(disco.Error))
            {
                throw new Exception(disco.Error);
            }

            var response = await httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "weatherApi",
                ClientSecret = "Mati",
                Code = "",
                CodeVerifier = "",
                RedirectUri = ""
            });
            if (response.IsError) throw new Exception(response.Error);
            return response.IdentityToken;
        }
    }
}