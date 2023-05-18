using System;
using Domain.BookRelated;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.BookRelated
{
    public class BookDboConfig : DboConfigBase<BookDbo, Guid>
    {
        public override void Configure(EntityTypeBuilder<BookDbo> builder)
        {
            base.Configure(builder);

            _ = builder.ToTable(nameof(SampleDbContext.Books), b => b.IsTemporal());
        }
    }
}
