using System.ComponentModel.DataAnnotations;
using Domain.BookRelated;
using Domain.Dtos;
using Domain.Persistance;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebApi.BookRelated.CommandAndQuery
{
    public static class GetPageOfBooks
    {
        public class Query : IRequest<Page<BookDto>>
        {
            [Required]
            public required int ItemsPerPage { get; set; }
            [Required]
            public required int PageIndex { get; set; }
            [Required]
            public required SearchMode SearchMode { get; set; }

            public IReadOnlyList<Guid>? Guids { get; set; }
            public string? TitleContains { get; set; }
        }

        public enum SearchMode
        {
            Guids = 0,
            TitleContains = 1,
            All = 100
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                _ = RuleFor(x => x.SearchMode).IsInEnum();
                _ = RuleFor(x => x.ItemsPerPage).GreaterThanOrEqualTo(1);
                _ = RuleFor(x => x.ItemsPerPage).LessThanOrEqualTo(100);
                _ = RuleFor(x => x.PageIndex).GreaterThanOrEqualTo(0);

                _ = RuleFor(x => x.Guids).NotEmpty().When(query => query.SearchMode == SearchMode.Guids);
                _ = RuleFor(x => x.Guids).ForEach(guid => guid.NotEmpty()).When(query => query.SearchMode == SearchMode.Guids);

                _ = RuleFor(x => x.TitleContains).NotEmpty().When(query => query.SearchMode == SearchMode.TitleContains);
            }
        }

        public class Handler : IRequestHandler<Query, Page<BookDto>>
        {
            private readonly IBookRepository bookRepository;
            private readonly IDtoMapper<Book, BookDto, Guid> bookDtoMapper;
            public Handler(IBookRepository bookRepository, IDtoMapper<Book, BookDto, Guid> bookDtoMapper)
            {
                this.bookRepository = bookRepository;
                this.bookDtoMapper = bookDtoMapper;
            }
            public async Task<Page<BookDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                // TODO LOW consider using cache

                var books = request.SearchMode switch
                {
                    SearchMode.Guids => await bookRepository.ListAsync(bookDbo => request.Guids!.Contains(bookDbo.UniqueId), cancellationToken, request.PageIndex, request.ItemsPerPage),
                    SearchMode.TitleContains => await bookRepository.ListAsync(bookDbo => bookDbo.Title.Contains(request.TitleContains!), cancellationToken, request.PageIndex, request.ItemsPerPage),
                    SearchMode.All => await bookRepository.ListAsync(bookDbo => true, cancellationToken, request.PageIndex, request.ItemsPerPage),

                    _ => throw new InvalidOperationException()
                };

                var bookDtos = new Page<BookDto>
                (
                    items: books.Items.Select(bookDtoMapper.ToDto).ToList()!,
                    pageIndex: books.PageIndex,
                    maxPageIndex: books.MaxPageIndex,
                    totalItems: books.TotalItems
                );

                return bookDtos;
            }
        }
    }
}
