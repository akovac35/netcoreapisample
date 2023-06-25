using System;
using System.Threading;
using System.Threading.Tasks;
using Domain.AuthorRelated;
using FluentValidation;
using MediatR;

namespace Infrastructure.AuthorRelated.CommandAndQuery
{
    public static class GetAuthor
    {
        public class Query : IRequest<AuthorDbo?>
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

        public class Handler : IRequestHandler<Query, AuthorDbo?>
        {
            private readonly IAuthorRepository authorRepository;

            public Handler(IAuthorRepository authorRepository)
            {
                this.authorRepository = authorRepository;
            }

            public async Task<AuthorDbo?> Handle(Query request, CancellationToken cancellationToken)
            {
                var dbo = await authorRepository.GetAsync(dbo => dbo.UniqueId == request.Id, cancellationToken);
                return dbo;
            }
        }
    }
}
