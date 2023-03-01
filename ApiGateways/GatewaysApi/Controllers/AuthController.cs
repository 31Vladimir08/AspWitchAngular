using GatewaysApi.ViewModels;

using IdentityModel.Client;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewaysApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        public LoginController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginVm userLogin)
        {
            using (var client = _clientFactory.CreateClient("IdentityService"))
            {
                var disco = await client.GetDiscoveryDocumentAsync();
                var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = disco.TokenEndpoint,

                    ClientId = "client",
                    ClientSecret = "secret",
                    Scope = "api1",

                    UserName = userLogin.Login,
                    Password = userLogin.Password
                });
                if (string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                {
                    return NotFound("user not found");
                }

                var response = await client.GetAsync($"api/Auth?login={userLogin.Login}");
                var userVm = await response.Content.ReadAsAsync<UserVm>();
                userVm.Token = tokenResponse.AccessToken;
                return Ok(userVm);
            }
        }
    }
}
