using System.Net;

using IdentityService.DbContext;
using IdentityService.Models;
using IdentityService.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        //private readonly IIdentityServerInteractionService _interaction;
        //private readonly IClientStore _clientStore;
        //private readonly IAuthenticationSchemeProvider _schemeProvider;
        //private readonly IEventService _events;
        private readonly SignInManager<ApplicationUser> _userManager;
        private readonly AuthDbContext _authDbContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            /*IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,*/
            SignInManager<ApplicationUser> userManager, 
            AuthDbContext authDbContext, 
            RoleManager<IdentityRole> roleManager)
        {
            /*_interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;*/

            _userManager = userManager;
            _authDbContext = authDbContext;
            _roleManager = roleManager;
        }

        [AllowAnonymous]
        [Route("SignIn")]
        [HttpPost]
        [ProducesResponseType(typeof(UserVm), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SignIn(LoginVm model)
        {
            // find user by username
            var user = await _userManager.UserManager.FindByNameAsync(model.Username);

            // validate username/password using ASP.NET Identity
            if (user is null || (await _userManager.CheckPasswordSignInAsync(user, model.Password, false)) !=
                Microsoft.AspNetCore.Identity.SignInResult.Success)
            {
                return Unauthorized();
            }

            var userVm = await _authDbContext.UserRoles.AsNoTracking()
                .Join(_authDbContext.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new UserVm()
                    {
                        UserId = ur.UserId,
                        DisplayName = user.DisplayName,
                        UserName = user.UserName,
                        RoleCode = r.NormalizedName
                    })
                .FirstOrDefaultAsync(x => x.UserName == user.UserName);
                
            return Ok(userVm);
        }

        [HttpPost]
        [Route("SignUp")]
        [ProducesResponseType(typeof(UserVm), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateUser([FromBody] RegistrationVm user)
        {
            var userExist = await _userManager.UserManager.FindByNameAsync(user.UserName);
            if (userExist is not null)
            {
                return BadRequest($"User {user.UserName} exists");
            }

            var role = await _roleManager.FindByNameAsync(user.RoleCode);
            if (role is null)
            {
                return BadRequest();
            }

            var newUser = new ApplicationUser
            {
                Email = user.Email,
                UserName = user.UserName,
                DisplayName = user.DisplayName
            };

            await using (var trans = await _authDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var userResult = await _userManager.UserManager.CreateAsync(newUser, user.Password);
                    if (!userResult.Succeeded)
                    {
                        await trans.RollbackAsync();
                        return BadRequest(userResult.Errors);
                    }

                    var userRole = await _userManager.UserManager.AddToRoleAsync(newUser, role.Name);
                    if (!userRole.Succeeded)
                    {
                        await trans.RollbackAsync();
                        return BadRequest(userResult.Errors);
                    }

                    await trans.CommitAsync();
                }
                catch (Exception e)
                {
                    await trans.RollbackAsync();
                    throw;
                }
            }

            var newUserVm = await _authDbContext.Users.AsNoTracking()
                .Join(_authDbContext.UserRoles,
                    x => x.Id,
                    ur => ur.UserId,
                    (x, ur) => new
                    {
                        UserId = x.Id,
                        DisplayName = x.DisplayName,
                        UserName = x.UserName,
                        RoleId = ur.RoleId
                    })
                .Join(_authDbContext.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new UserVm()
                    {
                        UserId = ur.UserId,
                        DisplayName = ur.DisplayName,
                        UserName = ur.UserName,
                        RoleCode = r.Name
                    })
                .FirstOrDefaultAsync(x => x.DisplayName == user.DisplayName);
            return Ok(newUserVm);
        }
    }
}
