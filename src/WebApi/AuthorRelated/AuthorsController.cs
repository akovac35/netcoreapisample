using System.ComponentModel.DataAnnotations;
using Domain;
using Domain.AuthorRelated;
using Domain.Dtos;
using Infrastructure.AuthorRelated.CommandAndQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.AuthorRelated
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class AuthorsController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IDtoMapper<AuthorDbo, AuthorDto, Guid> authorMapper;

        public AuthorsController(IMediator mediator, IDtoMapper<AuthorDbo, AuthorDto, Guid> authorMapper)
        {
            this.mediator = mediator;
            this.authorMapper = authorMapper;
        }

        [HttpGet("getAuthor/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthorDto?>> GetAuthor([Required] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetAuthor.Query(id);
            var dto = authorMapper.ToDto(await mediator.Send(query, cancellationToken));

            return Ok(dto);
        }

        [HttpPut("updateAuthor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AuthorDto>> UpdateAuthor([Required][FromBody] UpdateAuthor.Command command, CancellationToken cancellationToken)
        {
            var dto = authorMapper.ToDto(await mediator.Send(command, cancellationToken));

            return Ok(dto);
        }

        [HttpPost("createAuthor")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthorDto>> CreateAuthor([Required][FromBody] CreateAuthor.Command command, CancellationToken cancellationToken)
        {
            var dto = authorMapper.ToDto(await mediator.Send(command, cancellationToken));
            return CreatedAtAction(nameof(GetAuthor), new { id = dto.Id }, dto);
        }

        [HttpDelete("deleteAuthor/{id}")]
        [Authorize(Policy = Constants.RequireAdminRolePolicyKey)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> DeleteAuthor([Required] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteAuthor.Command(id);
            var nrDeleted = await mediator.Send(command, cancellationToken);
            return Ok(nrDeleted);
        }
    }
}
