using System.ComponentModel.DataAnnotations;
using Domain.BookRelated;
using Domain.Dtos;
using FluentValidation;
using MediatR;

namespace WebApi.BookRelated.CommandAndQuery
{
    public static class CreateBook
    {
        public class Command : IRequest<BookDto>
        {
            [Required]
            public required string Title { get; set; }
            [Required]
            public required string Author { get; set; }
            [Required]
            public required int Year { get; set; }
            [Required]
            public required string Publisher { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                _ = RuleFor(x => x.Title).NotEmpty();
                _ = RuleFor(x => x.Author).NotEmpty();
                _ = RuleFor(x => x.Publisher).NotEmpty();
                _ = RuleFor(x => x.Year).GreaterThanOrEqualTo(2000);
            }
        }

        public class Handler : IRequestHandler<Command, BookDto>
        {
            private readonly IBookRepository bookRepository;
            private readonly IDtoMapper<Book, BookDto, Guid> bookDtoMapper;
            public Handler(IBookRepository bookRepository, IDtoMapper<Book, BookDto, Guid> bookDtoMapper)
            {
                this.bookRepository = bookRepository;
                this.bookDtoMapper = bookDtoMapper;
            }
            public async Task<BookDto> Handle(Command request, CancellationToken cancellationToken)
            {
                var book = new Book(request.Title, request.Author, request.Year, request.Publisher);
                book = await bookRepository.AddAsync(book, cancellationToken);
                var bookDto = bookDtoMapper.ToDto(book);

                return bookDto;
            }
        }
    }
}
