using System.ComponentModel.DataAnnotations;
using Domain;
using Domain.BookRelated;
using Domain.Persistance;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.BookRelated.CommandAndQuery;

namespace WebApi.BookRelated
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IMediator mediator;

        public BooksController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("get/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookDto?>> GetBook([Required] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetBook.Query(id);
            var bookDto = await mediator.Send(query, cancellationToken);

            return Ok(bookDto);
        }

        [HttpPost("getPage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Page<BookDto>>> GetPageOfBooks([FromBody] GetPageOfBooks.Query query, CancellationToken cancellationToken)
        {
            var page = await mediator.Send(query, cancellationToken);
            return Ok(page);
        }

        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDto>> UpdateBook([FromBody] UpdateBook.Command command, CancellationToken cancellationToken)
        {
            var bookDto = await mediator.Send(command, cancellationToken);

            return Ok(bookDto);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBook.Command command, CancellationToken cancellationToken)
        {
            var bookDto = await mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetBook), new { id = bookDto.Id }, bookDto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Policy = Constants.RequireAdminRolePolicyKey)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> DeleteBook([Required] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteBook.Command(id);
            var nrDeleted = await mediator.Send(command, cancellationToken);
            return Ok(nrDeleted);
        }
    }
}
