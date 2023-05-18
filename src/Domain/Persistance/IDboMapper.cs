using System;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Persistance
{
    public interface IDboMapper<T, TDbo, TUniqueId>
        where T : IDboMappable<TUniqueId>
        where TDbo : DboBase<TUniqueId>
        where TUniqueId : notnull, IEquatable<TUniqueId>
    {
        [return: NotNullIfNotNull(nameof(dbo))]
        T? FromDbo(TDbo? dbo);

        [return: NotNullIfNotNull(nameof(value))]
        TDbo? ToDbo(T? value);

        void ToExistingDbo(T value, ref TDbo existingDbo);
    }
}
