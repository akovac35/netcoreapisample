using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos
{
    public abstract class DtoBase<TUniqueId> where TUniqueId : notnull, IEquatable<TUniqueId>
    {
        [Required]
        public required TUniqueId Id { get; set; } = default!;
    }
}
