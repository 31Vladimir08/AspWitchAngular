using System.Net;

using IdentityService.DbContext;
using IdentityService.ModelDto;
using IdentityService.Models;
using IdentityService.ViewModels;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AuthDbContext _authDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(AuthDbContext authDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _authDbContext = authDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        
        [HttpGet("{login}")]
        [ProducesResponseType(typeof(UserVm), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUser(string login)
        {
            var user = await _authDbContext.Users.AsNoTracking()
                .Join(_authDbContext.UserRoles,
                    x => x.Id,
                    ur => ur.UserId,
                    (x, ur) => new
                    {
                        UserId = x.Id,
                        Login = x.Login,
                        UserName = x.UserName,
                        RoleId = ur.RoleId
                    })
                .Join(_authDbContext.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new UserVm()
                    {
                        UserId = ur.UserId,
                        Login = ur.Login,
                        UserName = ur.UserName,
                        RoleCode = r.Name
                    })
                .FirstOrDefaultAsync(x => x.Login == login);
            if (user is null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserVm), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateUser([FromBody] ApplicationUserDto user)
        {
            var role = await _roleManager.FindByNameAsync(user.RoleCode);
            if (role is null)
            {
                return BadRequest();
            }

            var newUser = new ApplicationUser
            {
                Email = user.Email,
                UserName = user.UserName,
                Login = user.Login
            };

            using (var trans = _authDbContext.Database.BeginTransaction())
            {
                try
                {
                    var userResult = await _userManager.CreateAsync(newUser, user.Password);
                    if (!userResult.Succeeded)
                    {
                        trans.Rollback();
                        return BadRequest(userResult.Errors);
                    }

                    var userRole = await _userManager.AddToRoleAsync(newUser, role.Name);
                    if (!userRole.Succeeded)
                    {
                        trans.Rollback();
                        return BadRequest(userResult.Errors);
                    }

                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
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
                        Login = x.Login,
                        UserName = x.UserName,
                        RoleId = ur.RoleId
                    })
                .Join(_authDbContext.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new UserVm()
                    {
                        UserId = ur.UserId,
                        Login = ur.Login,
                        UserName = ur.UserName,
                        RoleCode = r.Name
                    })
                .FirstOrDefaultAsync(x => x.Login == user.Login);
            return Ok(newUserVm);
        }
    }
}
