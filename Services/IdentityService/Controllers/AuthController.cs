using IdentityService.DbContext;
using IdentityService.ViewModels;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _authDbContext;
        public AuthController(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
        }
        
        [HttpGet]
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
                        Role = r.Name
                    })
                .FirstOrDefaultAsync(x => x.UserName == login);
            if (user is null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
