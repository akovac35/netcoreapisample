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

            // We update row version outside db before saving an updated dbo. So the computed column
            // will be updated only when a changed dbo was saved
            _ = builder.Property(b => b.DbComputedColumn)
                .HasComputedColumnSql($"\"{nameof(BookDbo.RowVersion)}\" + 1");

            _ = builder.Property(b => b.DbDefaultValueColumn)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

        }
    }
}
