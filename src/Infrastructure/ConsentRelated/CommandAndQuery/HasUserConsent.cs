using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.ConsentRelated;
using FluentValidation;
using MediatR;

namespace Infrastructure.ConsentRelated.CommandAndQuery
{
    public static class HasUserConsent
    {
        public class Query : IRequest<IReadOnlyList<bool>>
        {
            [Required]
            public required string UserId { get; set; }
            [Required]
            public required IReadOnlyList<ConsentType> ConsentTypes { get; set; }
            public DateTimeOffset? ProbeTimestamp { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                _ = RuleFor(x => x.UserId).NotEmpty();
                _ = RuleFor(x => x.ConsentTypes).NotEmpty();
                _ = RuleForEach(x => x.ConsentTypes).IsInEnum();
            }
        }

        public class Handler : IRequestHandler<Query, IReadOnlyList<bool>>
        {
            private readonly IConsentRepository consentRepository;

            public Handler(IConsentRepository consentRepository) 
            {
                this.consentRepository = consentRepository;
            }

            public async Task<IReadOnlyList<bool>> Handle(Query request, CancellationToken cancellationToken)
            {
                var now = request.ProbeTimestamp?.ToUniversalTime() ?? DateTimeOffset.UtcNow;

                var page = await consentRepository.ListAsync(dbo => 
                    dbo.UserId == request.UserId 
                    && request.ConsentTypes.Contains(dbo.ConsentType)
                    && dbo.Revoked == false
                    && dbo.ValidFromUtc.CompareTo(now) <= 0
                    && dbo.ValidThroughUtc.CompareTo(now) >= 0, cancellationToken);

                return request.ConsentTypes
                    .Select(item => page.Items.Any(pageItem => pageItem.ConsentType == item))
                    .ToList();
            }
        }
    }
}
