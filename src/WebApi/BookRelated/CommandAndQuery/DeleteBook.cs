using Domain.BookRelated;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace WebApi.BookRelated.CommandAndQuery
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
                cache.Remove(request.Id);
                
                var nrDeleted = await bookRepository.DeleteAsync(bookDbo => bookDbo.UniqueId == request.Id, cancellationToken);

                return nrDeleted;
            }
        }
    }
}
