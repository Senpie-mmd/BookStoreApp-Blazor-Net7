using BookStoreApp.API.Data;
using BookStoreApp.API.Utilitis;
using BookStoreApp.API.ViewModels.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region Fields
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        #endregion

        #region Ctor
        public AuthController(ILogger<AuthController> logger,
            UserManager<ApiUser> userManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;
        }
        #endregion

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            try
            {
                if (model is null)
                {
                    return BadRequest("Invalid information.");
                }

                var user = new ApiUser()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded == false)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                        return BadRequest(ModelState);
                    }
                }

                await _userManager.AddToRoleAsync(user, "User");
                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(Register)}");
                return StatusCode(500, Message.Error500Message);
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<LoginResponseViewModel>> Login(LoginViewModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is null)
                    return NotFound();

                var passwordResult = await _userManager.CheckPasswordAsync(user, model.Password);
                if (passwordResult == false)
                    return Unauthorized();

                string tokenString = await GenerateJwtToken(user);

                var loginResponse = new LoginResponseViewModel()
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Token = tokenString
                };

                return loginResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Happend Getting {nameof(Login)}");
                return StatusCode(500, Message.Error500Message);
            }
        }

        private async Task<string> GenerateJwtToken(ApiUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userRoles = await _userManager.GetRolesAsync(user);
            var roleClaims = userRoles.Select(n => new Claim(ClaimTypes.Role, n)).ToList();

            var userClaims = await _userManager.GetClaimsAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(Convert.ToInt32(_configuration["JwtSettings:Duration"])),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
