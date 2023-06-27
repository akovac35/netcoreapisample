using System.ComponentModel.DataAnnotations;
using Domain;
using Domain.BookRelated;
using Domain.Dtos;
using Domain.Persistance;
using Infrastructure.BookRelated.CommandAndQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.BookRelated
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public class BooksController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly IDtoMapper<BookDbo, BookDto, Guid> bookMapper;

        public BooksController(IMediator mediator, IDtoMapper<BookDbo, BookDto, Guid> bookMapper)
        {
            this.mediator = mediator;
            this.bookMapper = bookMapper;
        }

        [HttpGet("getBook/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<BookDto?>> GetBook([Required] Guid id, CancellationToken cancellationToken)
        {
            var query = new GetBook.Query(id);
            var dto = bookMapper.ToDto(await mediator.Send(query, cancellationToken));

            return Ok(dto);
        }

        [HttpPost("getPageOfBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Page<BookDto>>> GetPageOfBooks([Required][FromBody] GetPageOfBooks.Query query, CancellationToken cancellationToken)
        {
            var bookDbos = await mediator.Send(query, cancellationToken);
            var bookDtos = new Page<BookDto>
            (
                items: bookDbos.Items.Select(bookMapper.ToDto).ToList()!,
                pageIndex: bookDbos.PageIndex,
                maxPageIndex: bookDbos.MaxPageIndex,
                totalItems: bookDbos.TotalItems
            );
            return Ok(bookDtos);
        }

        [HttpPut("updateBook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<BookDto>> UpdateBook([Required][FromBody] UpdateBook.Command command, CancellationToken cancellationToken)
        {
            var dto = bookMapper.ToDto(await mediator.Send(command, cancellationToken));

            return Ok(dto);
        }

        [HttpPost("createBook")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<BookDto>> CreateBook([Required][FromBody] CreateBook.Command command, CancellationToken cancellationToken)
        {
            var dto = bookMapper.ToDto(await mediator.Send(command, cancellationToken));
            return CreatedAtAction(nameof(GetBook), new { id = dto.Id }, dto);
        }

        [HttpDelete("deleteBook/{id}")]
        [Authorize(Policy = Constants.RequireAdminRolePolicyKey)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> DeleteBook([Required] Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteBook.Command(id);
            var nrDeleted = await mediator.Send(command, cancellationToken);
            return Ok(nrDeleted);
        }
    }
}
