using System;
using Domain.AuthorRelated;
using Domain.BookRelated;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.AuthorRelated
{
    public class AuthorDboConfig : DboConfigBase<AuthorDbo, Guid>
    {
        public override void Configure(EntityTypeBuilder<AuthorDbo> builder)
        {
            base.Configure(builder);

            _ = builder.ToTable(nameof(SampleDbContext.Authors), b => b.IsTemporal());

            _ = builder.HasMany<BookDbo>()
                .WithOne(b => b.Author)
                .IsRequired(false)
                // Sets foreign key in book table to null
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
