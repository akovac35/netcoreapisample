using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Domain.AuthorRelated;
using FluentValidation;
using MediatR;

namespace Infrastructure.AuthorRelated.CommandAndQuery
{
    public static class CreateAuthor
    {
        public class Command : IRequest<AuthorDbo>
        {
            [Required]
            public required string FirstName { get; set; }
            [Required]
            public required string LastName { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                _ = RuleFor(x => x.FirstName).NotEmpty();
                _ = RuleFor(x => x.LastName).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, AuthorDbo>
        {
            private readonly IAuthorRepository authorRepository;

            public Handler(IAuthorRepository authorRepository)
            {
                this.authorRepository = authorRepository;
            }
            public async Task<AuthorDbo> Handle(Command request, CancellationToken cancellationToken)
            {
                var dbo = new AuthorDbo
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UniqueId = Guid.NewGuid()
                };

                _ = await authorRepository.AddOrUpdateAsync(dbo, cancellationToken);

                return dbo;
            }
        }
    }
}
