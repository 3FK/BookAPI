using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Bookstore_API.Contracts;
using Bookstore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Bookstore_API.Controllers
{
    /// <summary>
    /// Endpoint for Users
    /// </summary>
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;

        public UsersController(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ILoggerService logger,
            IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _config = config;
        }
        /// <summary>
        /// Endpoint for User registration
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("registration")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            var location = GetController();
            try
            {
                _logger.LogInfo($"{location}: User attempted to Register");

                var UserName = userDTO.UserName;
                var EmailAddress = userDTO.EmailAddress;
                var Password = userDTO.Password;

                var user = new IdentityUser
                {
                    Email = EmailAddress,
                    UserName = UserName
                };
                var result = await _userManager.CreateAsync(user, Password);

                if (!result.Succeeded)
                {
                    foreach(var error in result.Errors)
                    {
                        _logger.LogError($"{location}: {error.Code} - {error.Description}");
                    }
                    return InternalError($"{location}: {UserName} - Registration attempt if faield");
                }
                //if (result.Succeeded)
                //{
                //    await _userManager.AddToRoleAsync("Customer");
                //}
                _logger.LogInfo($"{location}: User: {UserName} Registration success");
                await _userManager.AddToRoleAsync(user, "Customer");
                return Created("login",new { result.Succeeded });
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Endpoint for user Login
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns>User</returns>
        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var location = GetController();
            try
            {
                _logger.LogInfo($"{location}: User attemted to Login");

                var UserName = userDTO.UserName;
                var Password = userDTO.Password;

                var result = await _signInManager.PasswordSignInAsync(UserName, Password, false, false);

                if (result.Succeeded)
                {
                    _logger.LogInfo($"{location}: User: {UserName} successfully Login");
                    var user = await _userManager.FindByNameAsync(UserName);
                    var tokenString = generateJSONWebToken(user);
                    return Ok(new { token = tokenString.Result});
                }
                _logger.LogWarn($"{location}: User: {UserName} authenticated");
                return Unauthorized(userDTO);
        }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        private async Task<string> generateJSONWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                null,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token).ToString();
        }

        private string GetController()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";

        }
        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Somehing went wrong. Please contact the Administrator ");
        }
    }
}
