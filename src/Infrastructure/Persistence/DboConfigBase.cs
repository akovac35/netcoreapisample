using System;
using Domain.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence
{
    public abstract class DboConfigBase<TDbo, TUniqueId> : IEntityTypeConfiguration<TDbo>
        where TDbo : DboBase<TUniqueId>
        where TUniqueId : notnull, IEquatable<TUniqueId>
    {
        public virtual void Configure(EntityTypeBuilder<TDbo> builder)
        {
            _ = builder
                .HasIndex(b => b.UniqueId)
                .IsUnique();

            _ = builder
                .Property(b => b.RowVersion)
                .IsConcurrencyToken();
        }
    }
}
