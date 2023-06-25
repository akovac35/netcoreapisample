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
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace Infrastructure.BookRelated.CommandAndQuery
{
    public static class UpdateBook
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
            [Required]
            public required Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                _ = RuleFor(x => x.Title).NotEmpty();
                _ = RuleFor(x => x.AuthorUniqueId).NotEmpty();
                _ = RuleFor(x => x.Publisher).NotEmpty();
                _ = RuleFor(x => x.Year).GreaterThanOrEqualTo(2000);
                _ = RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, BookDbo>
        {
            private readonly IBookRepository bookRepository;
            private readonly IMemoryCache cache;
            private readonly IStringLocalizer<InfrastructureText> localizer;
            private readonly IMediator mediator;

            public Handler(IBookRepository bookRepository, IMemoryCache cache, IStringLocalizer<InfrastructureText> localizer, IMediator mediator)
            {
                this.bookRepository = bookRepository;
                this.cache = cache;
                this.localizer = localizer;
                this.mediator = mediator;
            }
            public async Task<BookDbo> Handle(Command request, CancellationToken cancellationToken)
            {
                var dbo = await bookRepository.GetAsync(dbo => dbo.UniqueId == request.Id, cancellationToken);

                if (dbo == null)
                {
                    throw new SampleNotFoundException(localizer["DboNotFound", nameof(BookDbo), request.Id].Value);
                }

                var authorDbo = await mediator.Send(new GetAuthor.Query(request.AuthorUniqueId), cancellationToken);

                if (authorDbo == null)
                {
                    throw new SampleNotFoundException(localizer["DboNotFound", nameof(AuthorDbo), request.AuthorUniqueId].Value);
                }

                dbo.Title = request.Title;
                dbo.Author = authorDbo;
                dbo.Year = request.Year;
                dbo.Publisher = request.Publisher;

                _ = await bookRepository.AddOrUpdateAsync(dbo, cancellationToken);

                cache.Remove(request.Id);

                return dbo;
            }
        }
    }
}
