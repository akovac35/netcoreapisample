using System.ComponentModel.DataAnnotations;
using Domain.BookRelated;
using Domain.Dtos;
using Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using WebApi.Resources;

namespace WebApi.BookRelated.CommandAndQuery
{
    public static class UpdateBook
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
            [Required]
            public required Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                _ = RuleFor(x => x.Title).NotEmpty();
                _ = RuleFor(x => x.Author).NotEmpty();
                _ = RuleFor(x => x.Publisher).NotEmpty();
                _ = RuleFor(x => x.Year).GreaterThanOrEqualTo(2000);
                _ = RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, BookDto>
        {
            private readonly IBookRepository bookRepository;
            private readonly IDtoMapper<Book, BookDto, Guid> bookDtoMapper;
            private readonly IMemoryCache cache;
            private readonly IStringLocalizer<WebApiText> localizer;

            public Handler(IBookRepository bookRepository, IDtoMapper<Book, BookDto, Guid> bookDtoMapper, IMemoryCache cache, IStringLocalizer<WebApiText> localizer)
            {
                this.bookRepository = bookRepository;
                this.bookDtoMapper = bookDtoMapper;
                this.cache = cache;
                this.localizer = localizer;
            }
            public async Task<BookDto> Handle(Command request, CancellationToken cancellationToken)
            {
                cache.Remove(request.Id);

                var book = await bookRepository.GetAsync(bookDbo => bookDbo.UniqueId == request.Id, cancellationToken);

                if (book == null)
                {
                    throw new SampleNotFoundException(localizer["UpdateBook_BookNotFound", request.Id].Value);
                }

                book.Title = request.Title;
                book.Author = request.Author;
                book.Year = request.Year;
                book.Publisher = request.Publisher;

                book = await bookRepository.UpdateAsync(book, cancellationToken);
                var bookDto = bookDtoMapper.ToDto(book);

                return bookDto;
            }
        }
    }
}
