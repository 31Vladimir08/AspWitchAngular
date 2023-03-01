using IdentityService.DbContext;

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

        [HttpGet]
        public IActionResult Index()
        {
            var t = _dbContext.Roles.ToList();
            return Ok();
        }
    }
}
