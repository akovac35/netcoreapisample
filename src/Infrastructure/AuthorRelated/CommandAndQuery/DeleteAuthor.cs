using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.AuthorRelated;
using FluentValidation;
using MediatR;

namespace Infrastructure.AuthorRelated.CommandAndQuery
{
    public static class DeleteAuthor
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
            private readonly IAuthorRepository authorRepository;

            public Handler(IAuthorRepository authorRepository)
            {
                this.authorRepository = authorRepository;
            }
            public async Task<int> Handle(Command request, CancellationToken cancellationToken)
            {
                var nrDeleted = await authorRepository.DeleteAsync(dbo => dbo.UniqueId == request.Id, cancellationToken);

                return nrDeleted;
            }
        }
    }
}
