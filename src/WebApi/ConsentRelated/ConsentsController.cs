using System.ComponentModel.DataAnnotations;
using Domain.ConsentRelated;
using Domain.Dtos;
using Infrastructure.ConsentRelated.CommandAndQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.ConsentRelated
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class ConsentsController: ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IDtoMapper<ConsentDbo, ConsentDto, Guid> consentMapper;

        public ConsentsController(IMediator mediator, IDtoMapper<ConsentDbo, ConsentDto, Guid> consentMapper)
        {
            this.mediator = mediator;
            this.consentMapper = consentMapper;
        }

        [HttpPost("getUserConsents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ConsentDto>>> GetUserConsents([Required][FromBody] GetUserConsents.Query query, CancellationToken cancellationToken)
        {
            var consents = await mediator.Send(query, cancellationToken);

            return Ok(consents.Select(item => consentMapper.ToDto(item)).ToList());
        }

        [HttpPost("hasUserConsent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<bool>>> HasUserConsent([Required][FromBody] HasUserConsent.Query query, CancellationToken cancellationToken)
        {
            var consents = await mediator.Send(query, cancellationToken);

            return Ok(consents);
        }

        [HttpPost("createConsent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2234:Pass system uri objects instead of strings")]
        public async Task<ActionResult<ConsentDto>> CreateConsent([Required][FromBody] CreateConsent.Command command, CancellationToken cancellationToken)
        {
            var dto = consentMapper.ToDto(await mediator.Send(command, cancellationToken));
            return Created(nameof(GetUserConsents), dto);
        }

        [HttpPut("updateConsent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ConsentDto>> UpdateConsent([Required][FromBody] UpdateConsent.Command command, CancellationToken cancellationToken)
        {
            var dto = consentMapper.ToDto(await mediator.Send(command, cancellationToken));

            return Ok(dto);
        }
    }
}
