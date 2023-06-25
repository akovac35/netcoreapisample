using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Domain.ConsentRelated;
using Domain.Exceptions;
using FluentValidation;
using Infrastructure.Resources;
using MediatR;
using Microsoft.Extensions.Localization;

namespace Infrastructure.ConsentRelated.CommandAndQuery
{
    public static class UpdateConsent
    {
        public class Command: IRequest<ConsentDbo>
        {
            [Required]
            public required Guid Id { get; set; }
            [Required]
            public required bool Revoked { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                _ = RuleFor(x => x.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Command, ConsentDbo>
        {
            private readonly IConsentRepository consentRepository;
            private readonly IStringLocalizer<InfrastructureText> localizer;

            public Handler(IConsentRepository consentRepository, IStringLocalizer<InfrastructureText> localizer)
            {
                this.consentRepository = consentRepository;
                this.localizer = localizer;
            }

            public async Task<ConsentDbo> Handle(Command request, CancellationToken cancellationToken)
            {
                var dbo = await consentRepository.GetAsync(dbo => dbo.UniqueId == request.Id, cancellationToken);

                if (dbo == null)
                {
                    throw new SampleNotFoundException(localizer["DboNotFound", nameof(ConsentDbo), request.Id].Value);
                }

                dbo.Revoked = request.Revoked;

                await consentRepository.AddOrUpdateAsync(dbo, cancellationToken);

                return dbo;
            }
        }
    }
}
