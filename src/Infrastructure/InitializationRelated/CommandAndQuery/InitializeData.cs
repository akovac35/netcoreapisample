using System.Threading;
using System.Threading.Tasks;
using Domain.ConsentRelated;
using FluentValidation;
using Infrastructure.AuthorRelated.CommandAndQuery;
using Infrastructure.BookRelated.CommandAndQuery;
using Infrastructure.ConsentRelated.CommandAndQuery;
using MediatR;

namespace Infrastructure.InitializationRelated.CommandAndQuery
{
    public static class InitializeData
    {
        public class Command : IRequest<Unit>
        {

        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
            }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            private readonly IMediator mediator;

            public Handler(IMediator mediator)
            {
                this.mediator = mediator;
            }

            public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
            {
                var createAuthorCommand = new CreateAuthor.Command
                {
                    FirstName = "George",
                    LastName = "Orwell"
                };

                var authorDbo = await mediator.Send(createAuthorCommand, cancellationToken);

                for (var i = 0; i < 20; i++)
                {
                    var createBookCommand = new CreateBook.Command
                    {
                        Title = $"Book {i}",
                        Publisher = $"Publisher {i}",
                        Year = 2000 + i,
                        AuthorUniqueId = authorDbo.UniqueId
                    };

                    _ = await mediator.Send(createBookCommand, cancellationToken);
                }

                var createConsentCommand = new CreateConsent.Command
                {
                    ConsentType = ConsentType.Telemarketing,
                    UserId = "alex",
                    ValidForNrDays = 10
                };

                _ = await mediator.Send(createConsentCommand, cancellationToken);

                return Unit.Value;
            }
        }
    }


}
