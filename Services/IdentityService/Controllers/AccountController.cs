﻿using System.Net;

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
        private readonly SignInManager<ApplicationUser> _userManager;
        private readonly AuthDbContext _authDbContext;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            SignInManager<ApplicationUser> userManager, 
            AuthDbContext authDbContext, 
            RoleManager<IdentityRole> roleManager)
        {
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

            var userVm = await GetUserAsync(user.UserName);

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
                        return BadRequest(new { Errors = userResult.Errors.Select(x => new { x.Code, x.Description }).ToList() });
                    }

                    var userRole = await _userManager.UserManager.AddToRoleAsync(newUser, role.Name);
                    if (!userRole.Succeeded)
                    {
                        await trans.RollbackAsync();
                        return BadRequest(new { Errors = userResult.Errors.Select(x => new { x.Code, x.Description}).ToList() });
                    }

                    await trans.CommitAsync();
                }
                catch (Exception e)
                {
                    await trans.RollbackAsync();
                    throw;
                }
            }

            var newUserVm = await GetUserAsync(user.UserName);
            return Ok(newUserVm);
        }

        [HttpGet]
        [Route("User")]
        [ProducesResponseType(typeof(UserVm), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUser(string username)
        {
            var userVm = await GetUserAsync(username);

            return Ok(userVm);
        }

        private async Task<UserVm?> GetUserAsync(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;
            var userVm = await _authDbContext.Users.AsNoTracking()
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
                        RoleCode = r.NormalizedName
                    })
                .FirstOrDefaultAsync(x => x.UserName == username);
            return userVm;
        }
    }
}
