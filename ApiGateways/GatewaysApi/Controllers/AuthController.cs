using System.Collections;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;

using GatewaysApi.ModelDto.Auth;
using GatewaysApi.Options.IdentityService;
using GatewaysApi.ViewModels.Auth;
using IdentityModel.Client;

using Microsoft.AspNetCore.Authentication;
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
        [ProducesResponseType(typeof(UserVm), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SignIn([FromBody]LoginVm userLogin)
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

            var tokenResponse = await GetTokenAsync(client, userLogin.Username, userLogin.Password);

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
                DisplayName = user.DisplayName,
                Password = user.Password,
                Email = user.Email,
                RoleCode = "USER"
            };

            using var client = _clientFactory.CreateClient(_identitySettingsOption.Name);
            var json = JsonSerializer.Serialize(userDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Account/SignUp", content);
                
            if (!response.IsSuccessStatusCode)
            {
                var exceptionContent = await response.Content.ReadAsStringAsync();
                var message = JObject.Parse(exceptionContent);
                return BadRequest(new {
                    Errors = new Hashtable()
                    {
                        {"Errors", message["errors"].ToString()}
                    }

                });
            }

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            var userVm = await JsonSerializer.DeserializeAsync<UserVm>(contentStream);
            
            var tokenResponse = await GetTokenAsync(client, userDto.UserName, userDto.Password);

            if (userVm is null || string.IsNullOrWhiteSpace(tokenResponse?.AccessToken))
            {
                return Unauthorized("user not found");
            }
                    
            userVm.Token = tokenResponse?.AccessToken;
            return Ok(userVm);
        }

        [Authorize]
        [Route("User")]
        [HttpGet]
        [ProducesResponseType(typeof(UserVm), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> GetCurrentUser()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            using var client = _clientFactory.CreateClient(_identitySettingsOption.Name);
            var disco = await client.GetDiscoveryDocumentAsync(_identitySettingsOption.IdentityServiceApi);
            var response = await client.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = accessToken
            });

            if (response is null)
                return Unauthorized("user not found");

            var username = response.Claims.FirstOrDefault(x => x.Type == "name")?.Value;
            
            var userResponse = await client.GetAsync($"/api/Account/User?username={username}");

            if (!userResponse.IsSuccessStatusCode)
            {
                var exceptionContent = await userResponse.Content.ReadAsStringAsync();
                var message = JObject.Parse(exceptionContent);
                return BadRequest(new
                {
                    Errors = new Hashtable()
                    {
                        {"Errors", message["errors"].ToString()}
                    }

                });
            }
            var userVm = await userResponse.Content.ReadAsAsync<UserVm>();
            userVm.Token = accessToken;
            return Ok(userVm);
        }

        private async Task<TokenResponse?> GetTokenAsync(HttpClient client, string username, string password)
        {
            var disco = await client.GetDiscoveryDocumentAsync();
            if (disco.IsError)
            {
                throw new Exception(disco.Error);
            }

            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,

                ClientId = _identitySettingsOption.ClientId,
                ClientSecret = _identitySettingsOption.ClientSecret,

                UserName = username,
                Password = password
            });

            return tokenResponse;
        }
    }
}
