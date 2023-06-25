using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.BookRelated;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.BookRelated.CommandAndQuery
{
    public static class GetBook
    {
        public class Query : IRequest<BookDbo?>
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

        public class Handler : IRequestHandler<Query, BookDbo?>
        {
            private readonly IBookRepository bookRepository;
            private readonly IMemoryCache cache;

            public Handler(IBookRepository bookRepository, IMemoryCache cache)
            {
                this.bookRepository = bookRepository;
                this.cache = cache;
            }
            public async Task<BookDbo?> Handle(Query request, CancellationToken cancellationToken)
            {
                return await cache.GetOrCreateAsync(request.Id, async cacheEntry =>
                {
                    cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(10);
                    var dbo = await HandleImpl(request, cancellationToken);
                    return dbo;
                });
            }

            private async Task<BookDbo?> HandleImpl(Query request, CancellationToken cancellationToken)
            {
                var dbo = await bookRepository.GetAsync(dbo => dbo.UniqueId == request.Id, cancellationToken);
                return dbo;
            }
        }
    }
}
