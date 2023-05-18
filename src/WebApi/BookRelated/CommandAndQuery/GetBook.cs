using Domain.BookRelated;
using Domain.Dtos;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace WebApi.BookRelated.CommandAndQuery
{
    public static class GetBook
    {
        public class Query : IRequest<BookDto>
        {
            public Query(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                _ = RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, BookDto?>
        {
            private readonly IBookRepository bookRepository;
            private readonly IDtoMapper<Book, BookDto, Guid> bookDtoMapper;
            private readonly IMemoryCache cache;

            public Handler(IBookRepository bookRepository, IDtoMapper<Book, BookDto, Guid> bookDtoMapper, IMemoryCache cache)
            {
                this.bookRepository = bookRepository;
                this.bookDtoMapper = bookDtoMapper;
                this.cache = cache;
            }
            public async Task<BookDto?> Handle(Query request, CancellationToken cancellationToken)
            {
                return await cache.GetOrCreateAsync(request.Id, async cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(10);
                    var bookDto = await HandleImpl(request, cancellationToken);
                    return bookDto;
                });
            }

            private async Task<BookDto?> HandleImpl(Query request, CancellationToken cancellationToken)
            {
                var book = await bookRepository.GetAsync(bookDbo => bookDbo.UniqueId == request.Id, cancellationToken);
                var bookDto = bookDtoMapper.ToDto(book);
                return bookDto;
            }
        }
    }
}
