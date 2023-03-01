using System.Security.Claims;
using IdentityModel;
using IdentityService.Interfaces.Initialazer;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.DbContext
{
    public class DbInitializer : IDbInitializer
    {
        private readonly AuthDbContext _authDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(AuthDbContext authDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _authDbContext = authDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            if (_roleManager.FindByNameAsync(nameof(RoleEnum.Administrator)).Result is null)
            {
                _roleManager.CreateAsync(new IdentityRole(nameof(RoleEnum.Administrator))).GetAwaiter().GetResult();
            }

            if (_roleManager.FindByNameAsync(nameof(RoleEnum.User)).Result is null)
            {
                _roleManager.CreateAsync(new IdentityRole(nameof(RoleEnum.User))).GetAwaiter().GetResult();
            }

            if (_userManager.FindByEmailAsync("admin@admin.com").Result is null)
            {
                var admin = new ApplicationUser
                {
                    Email = "admin@admin.com",
                    UserName = "admin@admin.com",
                    NormalizedUserName = "Admin",
                    Login = "admin@admin.com"
                };

                _userManager.CreateAsync(admin, "$Admin12345$").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(admin, nameof(RoleEnum.Administrator)).GetAwaiter().GetResult();
                var identityResult = _userManager.AddClaimsAsync(admin, new Claim[]
                {
                    new Claim(JwtClaimTypes.Name, admin.UserName),
                    new Claim(JwtClaimTypes.Role, nameof(RoleEnum.Administrator))
                }).Result;
            }
        }
    }
}
