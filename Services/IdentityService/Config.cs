using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityService
{
    public static class Config
    {
        public static IEnumerable<Client> GetClients() =>
        new List<Client>
        {
            new Client
            {
                ClientId = "GatewaysAPI",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                RequirePkce = false,
                AllowRememberConsent = false,

                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },
                AllowedScopes = new List<string>()
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email
                }
            }
        };


        public static IEnumerable<ApiResource> GetApiResources()
        {
            yield return new ApiResource("GatewaysAPI");
            //yield return new ApiResource("OrdersAPI");
            //return new List<ApiResource>();
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            yield return new IdentityResources.OpenId();
            yield return new IdentityResources.Profile();
        }
        
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            yield return new ApiScope("GatewaysAPI", "Gateways API");
            //yield return new ApiScope("blazor", "Blazor WebAssembly");
            //yield return new ApiScope("OrdersAPI", "Orders API");
            /*return new List<ApiScope>()
            {

            };*/
        }
    }
}
