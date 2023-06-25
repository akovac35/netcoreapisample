using System;
using Domain.ConsentRelated;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.ConsentRelated
{
    public class ConsentDboConfig : DboConfigBase<ConsentDbo, Guid>
    {
        public override void Configure(EntityTypeBuilder<ConsentDbo> builder)
        {
            base.Configure(builder);

            _ = builder.ToTable(nameof(SampleDbContext.Consents), b => b.IsTemporal());

            _ = builder.Property(e => e.ConsentType)
                .HasConversion<string>();
        }
    }
}
