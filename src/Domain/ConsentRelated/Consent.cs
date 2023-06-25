using System;
using System.ComponentModel.DataAnnotations;
using Domain.Dtos;
using Domain.Persistance;

namespace Domain.ConsentRelated
{
    #region Dbo

    public class ConsentDbo : DboBase<Guid>
    {
        public required ConsentType ConsentType { get; set; }
        public required DateTimeOffset ValidFromUtc { get; set; }
        public required DateTimeOffset ValidThroughUtc { get; set; }
        public required bool Revoked { get; set; }
        public required string UserId { get; set; }
    }

    #endregion

    #region Dto

    public class ConsentDto: DtoBase<Guid>
    {
        [Required]
        public required ConsentType ConsentType { get; set; }
        [Required]
        public required DateTimeOffset ValidFromUtc { get; set; }
        [Required]
        public required DateTimeOffset ValidThroughUtc { get; set; }
        [Required]
        public required bool Revoked { get; set; }
        [Required]
        public required string UserId { get; set; }
    }

    public class ConsentDtoMapper : IDtoMapper<ConsentDbo, ConsentDto, Guid>
    {
        public ConsentDto? ToDto(ConsentDbo? value)
        {
            return value == null ? null :
                new ConsentDto
                {
                    ConsentType = value.ConsentType,
                    ValidFromUtc = value.ValidFromUtc,
                    ValidThroughUtc = value.ValidThroughUtc,
                    Revoked = value.Revoked,
                    UserId = value.UserId,
                    Id = value.UniqueId
                };
        }

        public void ToExistingDto(ConsentDbo value, ref ConsentDto existingDto)
        {
            existingDto.ConsentType = value.ConsentType;
            existingDto.ValidFromUtc = value.ValidFromUtc;
            existingDto.ValidThroughUtc = value.ValidThroughUtc;
            existingDto.Revoked = value.Revoked;
            existingDto.UserId = value.UserId;
        }
    }

    #endregion
}
