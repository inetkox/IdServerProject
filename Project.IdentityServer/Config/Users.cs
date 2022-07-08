using IdentityModel;
using IdentityServer4.Test;
using System.Security.Claims;

namespace Project.IdentityServer.Config
{
    public class Users
    {
        public static List<TestUser> Get()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "mati",
                    Password = "szy",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Mateusz")
                    },
                }
            };
        }
    }
}
