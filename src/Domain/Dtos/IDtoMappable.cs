using System;

namespace Domain.Dtos
{
    public interface IDtoMappable<TUniqueId>
        where TUniqueId : notnull, IEquatable<TUniqueId>
    {
        TUniqueId DtoUniqueId { get; }
    }
}
