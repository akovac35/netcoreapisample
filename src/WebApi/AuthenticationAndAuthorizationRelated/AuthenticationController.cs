using System.ComponentModel.DataAnnotations;
using Domain.AuthenticationAndAuthorizationRelated;
using Infrastructure.AuthenticationAndAuthorizationRelated.CommandAndQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.AuthenticationAndAuthorizationRelated
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthenticationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("createAuthenticationToken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokenDto>> CreateAuthenticationToken([Required][FromBody] CreateAuthenticationToken.Command command, CancellationToken cancellationToken)
        {
            var token = await mediator.Send(command, cancellationToken);

            return Ok(new TokenDto { Token = token });
        }
    }
}
