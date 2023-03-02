using System.Net;

using AutoMapper;

using IdentityService.DbContext;
using IdentityService.ModelDto;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AuthDbContext _authDbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleController(AuthDbContext authDbContext, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _authDbContext = authDbContext;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        
        [HttpGet("{roleName}")]
        [ProducesResponseType(typeof(RoleDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                return BadRequest();
            }

            var roleDto = _mapper.Map<RoleDto>(role);
            return Ok(roleDto);
        }

        [HttpGet()]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetRoles()
        {
            var role = await _roleManager.Roles.ToListAsync();

            var rolesDto = _mapper.Map<IEnumerable<RoleDto>>(role);

            return Ok(rolesDto);
        }
    }
}
