using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Domain.ConsentRelated;
using FluentValidation;
using MediatR;

namespace Infrastructure.ConsentRelated.CommandAndQuery
{
    public static class CreateConsent
    {
        public class Command : IRequest<ConsentDbo>
        {
            [Required]
            public required ConsentType ConsentType { get; set; }
            [Required]
            public required int ValidForNrDays { get; set; }
            [Required]
            public required string UserId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                _ = RuleFor(x => x.ConsentType).IsInEnum();
                _ = RuleFor(x => x.ValidForNrDays).GreaterThanOrEqualTo(1);
                _ = RuleFor(x => x.UserId).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, ConsentDbo>
        {
            private readonly IConsentRepository consentRepository;

            public Handler(IConsentRepository consentRepository) 
            {
                this.consentRepository = consentRepository;
            }

            public async Task<ConsentDbo> Handle(Command request, CancellationToken cancellationToken)
            {
                var now = DateTimeOffset.UtcNow;

                var dbo = new ConsentDbo
                {
                    ConsentType = request.ConsentType,
                    ValidFromUtc = now,
                    ValidThroughUtc = now.AddDays(request.ValidForNrDays),
                    Revoked = false,
                    UserId = request.UserId,
                    UniqueId = Guid.NewGuid()
                };

                await consentRepository.AddOrUpdateAsync(dbo, cancellationToken);

                return dbo;
            }
        }
    }
}
