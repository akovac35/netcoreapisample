using System;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Dtos
{
    public interface IDtoMapper<T, TDto, TUniqueId>
        where T : IDtoMappable<TUniqueId>
        where TDto : DtoBase<TUniqueId>
        where TUniqueId : notnull, IEquatable<TUniqueId>
    {
        [return: NotNullIfNotNull(nameof(dto))]
        T? FromDto(TDto? dto);

        [return: NotNullIfNotNull(nameof(value))]
        TDto? ToDto(T? value);

        void ToExistingDto(T value, ref TDto existingDto);

        void ToExistingInstance(TDto dto, ref T existingInstance);
    }
}
