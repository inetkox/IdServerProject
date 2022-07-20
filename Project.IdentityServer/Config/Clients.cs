using IdentityServer4;
using IdentityServer4.Models;

namespace Project.IdentityServer.Config
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
        {
            new Client
            {
                ClientId = "weatherApi",
                ClientName = "ASP.NET Core Api",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowAccessTokensViaBrowser = true,
                ClientSecrets = new List<Secret> {new Secret("Mati".Sha256())},
                AllowedScopes = new List<string> {                         
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "role",
                        "weatherApi.read" },
                AllowOfflineAccess = true,
                RequireConsent = true,
                AlwaysIncludeUserClaimsInIdToken = true,

            },
            new Client
                {
                    ClientId = "mvc",
                    ClientName = "ASP.NET Core MVC Web App",
                    ClientSecrets = new List<Secret> {new Secret("Mati".Sha256())},

                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string> {"https://localhost:5003/signin-oidc"},
                    PostLogoutRedirectUris = { "https://localhost:5003/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "role",
                        "weatherApi.read"
                    },
                    
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    RequirePkce = true,
                    AllowPlainTextPkce = false,
                    RequireConsent = true,
                    AlwaysIncludeUserClaimsInIdToken = true
                }
        };
        }
    }
}
