using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using FluentValidation;
using Infrastructure.Resources;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.AuthenticationAndAuthorizationRelated.CommandAndQuery
{
    public static class CreateAuthenticationToken
    {
        public class Command : IRequest<string>
        {
            [Required]
            public required string Username { get; set; }
            [Required]
            public required string Password { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                _ = RuleFor(x => x.Username).NotEmpty();
                _ = RuleFor(x => x.Password).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, string>
        {
            private readonly IOptions<SampleConfig> config;
            private readonly IStringLocalizer<InfrastructureText> localizer;

            public Handler(IOptions<SampleConfig> config, IStringLocalizer<InfrastructureText> localizer)
            {
                this.config = config;
                this.localizer = localizer;
            }

            public Task<string> Handle(Command request, CancellationToken cancellationToken)
            {
                Claim roleClaim = null!;

                // TODO HIGH encrypt provided password with salt, then find value
                // in identity provider's database. Roles should also be
                // fetched from identity provider's database
                if (request.Username.ToUpper(Constants.InvariantCulture) == "ADMIN"
                    && request.Password == "password")
                {
                    roleClaim = new Claim(ClaimTypes.Role, Constants.AdminRoleKey);
                }
                else if (request.Username.ToUpper(Constants.InvariantCulture) == "USER"
                    && request.Password == "password")
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
                        new Claim(ClaimTypes.Name, request.Username),
                        roleClaim
                    })
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Task.FromResult(tokenString);
            }
        }
    }
}
