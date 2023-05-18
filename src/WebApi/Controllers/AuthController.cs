using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Resources;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<SampleConfig> config;
        private readonly IStringLocalizer<WebApiText> localizer;

        public AuthController(IOptions<SampleConfig> config, IStringLocalizer<WebApiText> localizer)
        {
            this.config = config;
            this.localizer = localizer;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto model)
        {
            Claim roleClaim = null!;

            if (model.Username.ToUpper(Constants.InvariantCulture) == "ADMIN"
                && model.Password == "password")
            {
                roleClaim = new Claim(ClaimTypes.Role, Constants.AdminRoleKey);
            }
            else if(model.Username.ToUpper(Constants.InvariantCulture) == "USER"
                && model.Password == "password")
            {
                roleClaim = new Claim(ClaimTypes.Role, Constants.UserRoleKey);
            }
            else
            {
                throw new UnauthorizedAccessException(localizer["Login_Invalid"].Value);
            }

            // Assuming the user is valid, generate a JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(config.Value.JwtConfig.JwtSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = Constants.ApplicationAcronym,
                Audience = Constants.JwtAudience,
                Expires = DateTime.UtcNow.AddMinutes(config.Value.JwtConfig.TokenValidityInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    roleClaim
                })
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }

    public class LoginDto
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
