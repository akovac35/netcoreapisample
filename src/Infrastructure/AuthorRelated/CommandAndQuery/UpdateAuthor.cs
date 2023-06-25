using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Domain.AuthorRelated;
using Domain.Exceptions;
using FluentValidation;
using Infrastructure.Resources;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Infrastructure.AuthorRelated.CommandAndQuery
{
    public static class UpdateAuthor
    {
        public class Command : IRequest<AuthorDbo>
        {
            [Required]
            public required string FirstName { get; set; }
            [Required]
            public required string LastName { get; set; }
            [Required]
            public required Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                _ = RuleFor(x => x.FirstName).NotEmpty();
                _ = RuleFor(x => x.LastName).NotEmpty();
                _ = RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, AuthorDbo>
        {
            private readonly IAuthorRepository authorRepository;
            private readonly IStringLocalizer<InfrastructureText> localizer;

            public Handler(IAuthorRepository authorRepository, IStringLocalizer<InfrastructureText> localizer)
            {
                this.authorRepository = authorRepository;
                this.localizer = localizer;
            }
            public async Task<AuthorDbo> Handle(Command request, CancellationToken cancellationToken)
            {
                var dbo = await authorRepository.GetAsync(dbo => dbo.UniqueId == request.Id, cancellationToken);

                if (dbo == null)
                {
                    throw new SampleNotFoundException(localizer["DboNotFound", nameof(AuthorDbo), request.Id].Value);
                }

                dbo.FirstName = request.FirstName;
                dbo.LastName = request.LastName;

                _ = await authorRepository.AddOrUpdateAsync(dbo, cancellationToken);

                return dbo;
            }
        }
    }
}
