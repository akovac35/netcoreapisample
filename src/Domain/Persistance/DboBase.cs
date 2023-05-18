using System;

namespace Domain.Persistance
{
    public interface IDboBase
    {
        void OnSavingModified();
    }

    public abstract class DboBase<TUniqueId> : IDboBase where TUniqueId : notnull, IEquatable<TUniqueId>
    {
        public int RowVersion { get; set; }

        public DateTime CreatedOnUtc { get; init; } = DateTime.UtcNow; // Note: https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations#specify-the-datetimekind-when-reading-dates

        public DateTime? ModifiedAtUtc { get; set; }

        public required TUniqueId UniqueId { get; set; } = default!;

        public void OnSavingModified()
        {
            ModifiedAtUtc = DateTime.UtcNow;
            RowVersion += 1;
        }

        public long Id { get; init; }
    }
}
