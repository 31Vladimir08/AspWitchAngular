using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityService.Models;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public AccountController(IIdentityServerInteractionService interactionService, IServerUrls serverUrls, ITokenCreationService tokenCreationService,
         SignInManager<ApplicationUser> signInManager)
        {
            InteractionService = interactionService;
            ServerUrls = serverUrls;
            SignInManager = signInManager;
            TokenCreationService = tokenCreationService;
        }

        private IIdentityServerInteractionService InteractionService { get; }

        private ITokenCreationService TokenCreationService { get; }

        private IServerUrls ServerUrls { get; }

        private SignInManager<ApplicationUser> SignInManager { get; }

        [HttpPost("/api/login")]
        public async Task<IActionResult> Login([FromBody]LoginVm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var t = await TokenCreationService.CreateTokenAsync(new Token(nameof(AccessTokenType.Jwt))
            {
                ClientId = "client",
                AccessTokenType = AccessTokenType.Jwt,
                CreationTime = DateTime.Now
                
            });
            var result = await SignInManager.PasswordSignInAsync(model.Login, model.Password, false, false);

            return result.Succeeded ? Ok() : BadRequest();
        }

        [HttpGet("/api/logout")]
        public async Task<IActionResult> Logout([FromQuery] string logoutId)
        {
            var logoutRequest = await InteractionService.GetLogoutContextAsync(logoutId);

            if (logoutRequest == null || (logoutRequest.ShowSignoutPrompt && User.Identity?.IsAuthenticated == true))
            {
                return Ok(new { prompt = User.Identity?.IsAuthenticated ?? false });
            }

            await SignInManager.SignOutAsync();

            return Ok(new
            {
                iFrameUrl = logoutRequest.SignOutIFrameUrl,
                postLogoutRedirectUri = logoutRequest.PostLogoutRedirectUri
            });
        }

        [HttpPost("/api/logout")]
        public async Task<IActionResult> PostLogout([FromQuery] string logoutId)
        {
            var logoutRequest = await InteractionService.GetLogoutContextAsync(logoutId);

            await SignInManager.SignOutAsync();

            return Ok(new
            {
                iFrameUrl = logoutRequest?.SignOutIFrameUrl,
                postLogoutRedirectUri = logoutRequest?.PostLogoutRedirectUri
            });
        }

        public record LoginRequestModel(string UserName, string Password, string? ReturnUrl, bool IsPersistent = false);
    }
}
