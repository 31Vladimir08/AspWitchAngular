using IdentityService.DbContext;
using IdentityService.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [ApiController]
    [Route("api/identity")]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _dbContext;
        public AuthController(AuthDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public IActionResult SignIn([FromBody]LoginVm login)
        {
            var currentUser = _dbContext.Users.FirstOrDefault(x => x.Login == login.Login && x.PasswordHash == login.Password);
            if (currentUser is null)
            {
                return NotFound("Wrong login or password");
            }

            return Ok();
        }
    }
}
