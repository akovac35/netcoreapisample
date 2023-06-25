using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Domain.AuthorRelated;
using Domain.BookRelated;
using Domain.Exceptions;
using FluentValidation;
using Infrastructure.AuthorRelated.CommandAndQuery;
using Infrastructure.Resources;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Infrastructure.BookRelated.CommandAndQuery
{
    public static class CreateBook
    {
        public class Command : IRequest<BookDbo>
        {
            [Required]
            public required string Title { get; set; }
            [Required]
            public required Guid AuthorUniqueId { get; set; }
            [Required]
            public required int Year { get; set; }
            [Required]
            public required string Publisher { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                _ = RuleFor(x => x.Title).NotEmpty();
                _ = RuleFor(x => x.AuthorUniqueId).NotEmpty();
                _ = RuleFor(x => x.Publisher).NotEmpty();
                _ = RuleFor(x => x.Year).GreaterThanOrEqualTo(2000);
            }
        }

        public class Handler : IRequestHandler<Command, BookDbo>
        {
            private readonly IBookRepository bookRepository;
            private readonly IMediator mediator;
            private readonly IStringLocalizer<InfrastructureText> localizer;

            public Handler(IBookRepository bookRepository, IMediator mediator, IStringLocalizer<InfrastructureText> localizer)
            {
                this.bookRepository = bookRepository;
                this.mediator = mediator;
                this.localizer = localizer;
            }
            public async Task<BookDbo> Handle(Command request, CancellationToken cancellationToken)
            {
                var authorDbo = await mediator.Send(new GetAuthor.Query(request.AuthorUniqueId), cancellationToken);

                if (authorDbo == null)
                {
                    throw new SampleNotFoundException(localizer["DboNotFound", nameof(AuthorDbo), request.AuthorUniqueId].Value);
                }

                var dbo = new BookDbo
                {
                    Title = request.Title,
                    Author = authorDbo,
                    Year = request.Year,
                    Publisher = request.Publisher,
                    UniqueId = Guid.NewGuid()
                };

                _ = await bookRepository.AddOrUpdateAsync(dbo, cancellationToken);

                return dbo;
            }
        }
    }
}
