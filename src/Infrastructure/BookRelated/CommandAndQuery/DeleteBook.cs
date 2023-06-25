using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.BookRelated;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.BookRelated.CommandAndQuery
{
    public static class DeleteBook
    {
        public class Command : IRequest<int>
        {
            public Command(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                _ = RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, int>
        {
            private readonly IBookRepository bookRepository;
            private readonly IMemoryCache cache;

            public Handler(IBookRepository bookRepository, IMemoryCache cache)
            {
                this.bookRepository = bookRepository;
                this.cache = cache;
            }
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var nrDeleted = await bookRepository.DeleteAsync(dbo => dbo.UniqueId == request.Id, cancellationToken);

                cache.Remove(request.Id);

                return nrDeleted;
            }
        }
    }
}
