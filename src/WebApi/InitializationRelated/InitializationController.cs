using System.ComponentModel.DataAnnotations;
using Infrastructure.InitializationRelated.CommandAndQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.InitializationRelated
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class InitializationController : ControllerBase
    {
        private readonly IMediator mediator;

        public InitializationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("initializeData")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> InitializeData([Required][FromBody] InitializeData.Command command, CancellationToken cancellationToken)
        {
            _ = await mediator.Send(command, cancellationToken);

            return Ok();
        }
    }
}
