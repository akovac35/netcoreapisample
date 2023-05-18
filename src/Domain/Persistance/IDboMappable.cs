using System;

namespace Domain.Persistance
{
    public interface IDboMappable<TUniqueId>
        where TUniqueId : notnull, IEquatable<TUniqueId>
    {
        TUniqueId DboUniqueId { get; }

        public int DboVersion { get; }
    }
}
