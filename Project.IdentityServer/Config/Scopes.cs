using IdentityServer4.Models;

namespace Project.IdentityServer.Config
{
    public class Scopes
    {
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
            new ApiScope("weatherApi.read", "Read Access to myOwnAPI"),
            new ApiScope("weatherApi.write", "Write Access to myOwnAPI"),
        };
        }
    }
}
