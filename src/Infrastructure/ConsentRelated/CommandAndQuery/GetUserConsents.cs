using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Domain.ConsentRelated;
using FluentValidation;
using MediatR;

namespace Infrastructure.ConsentRelated.CommandAndQuery
{
    public static class GetUserConsents
    {
        public class Query: IRequest<IReadOnlyList<ConsentDbo>>
        {
            [Required]
            public required string UserId { get; set; }
            public bool IncludeRevoked { get; set; }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                _ = RuleFor(x => x.UserId).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Query, IReadOnlyList<ConsentDbo>>
        {
            private readonly IConsentRepository consentRepository;

            public Handler(IConsentRepository consentRepository)
            {
                this.consentRepository = consentRepository;
            }

            public async Task<IReadOnlyList<ConsentDbo>> Handle(Query request, CancellationToken cancellationToken)
            {
                var now = DateTimeOffset.UtcNow;

                var page = await consentRepository.ListAsync(dbo => 
                    dbo.UserId == request.UserId
                    && dbo.ValidFromUtc.CompareTo(now) <= 0
                    && dbo.ValidThroughUtc.CompareTo(now) >= 0
                    && request.IncludeRevoked
                        ? true 
                        : dbo.Revoked == false, 
                cancellationToken);

                return page.Items;
            }
        }
    }
}
