using System.Net;
using System.Text;
using System.Text.Json;

using GatewaysApi.ModelDto.Auth;
using GatewaysApi.Options.IdentityService;
using GatewaysApi.ViewModels.Auth;
using IdentityModel.Client;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace GatewaysApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IdentitySettingsOption _identitySettingsOption;
        public AuthController(IHttpClientFactory clientFactory, IOptions<IdentitySettingsOption> identitySettingsOption)
        {
            _clientFactory = clientFactory;
            _identitySettingsOption = identitySettingsOption.Value;
        }

        [AllowAnonymous]
        [Route("SignIn")]
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] LoginVm userLogin)
        {
            using var client = _clientFactory.CreateClient(_identitySettingsOption.Name);
            var json = JsonSerializer.Serialize(userLogin);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Account/SignIn", content);
            if (!response.IsSuccessStatusCode)
            {
                var exceptionContent = await response.Content.ReadAsStringAsync();
                var message = JObject.Parse(exceptionContent);
                return Unauthorized("user not found");
            }

            var userVm = await response.Content.ReadAsAsync<UserVm>();
            var disco = await client.GetDiscoveryDocumentAsync();
            if (disco.IsError)
            {
                return BadRequest(disco.Error);
            }
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = _identitySettingsOption.ClientId,
                ClientSecret = _identitySettingsOption.ClientSecret,
                Scope = _identitySettingsOption.Scope,

                UserName = userLogin.Username,
                Password = userLogin.Password
            });

            if (string.IsNullOrWhiteSpace(tokenResponse?.AccessToken))
            {
                return Unauthorized("user not found");
            }

            userVm.Token = tokenResponse.AccessToken;
            return Ok(userVm);
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(UserVm), (int)HttpStatusCode.OK)]
        [Route("SignUp")]
        public async Task<IActionResult> SignUp([FromBody]RegistrationVm? user)
        {
            if (user is null)
            {
                return BadRequest();
            }

            var userDto = new UserDto()
            {
                UserName = user.UserName,
                Login = user.Login,
                Password = user.Password,
                Email = user.Email,
                RoleCode = "USER"
            };

            using var client = _clientFactory.CreateClient(_identitySettingsOption.Name);
            var json = JsonSerializer.Serialize(userDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/User", content);
                
            if (!response.IsSuccessStatusCode)
            {
                var exceptionContent = await response.Content.ReadAsStringAsync();
                var message = JObject.Parse(exceptionContent);
                return BadRequest(message["Message"].ToString());
            }

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            var userVm = await JsonSerializer.DeserializeAsync<UserVm>(contentStream);

            var disco = await client.GetDiscoveryDocumentAsync();
            if (disco.IsError)
            {
                return BadRequest(disco.Error);
            }

            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = _identitySettingsOption.ClientId,
                ClientSecret = _identitySettingsOption.ClientSecret,
                Scope = _identitySettingsOption.Scope,

                UserName = user.Login,
                Password = user.Password
            });

            if (userVm == null || string.IsNullOrWhiteSpace(tokenResponse?.AccessToken))
            {
                return Unauthorized("user not found");
            }
                    
            userVm.Token = tokenResponse?.AccessToken;
            return Ok(userVm);
        }
    }
}
