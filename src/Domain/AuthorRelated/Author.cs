using System;
using System.ComponentModel.DataAnnotations;
using Domain.Dtos;
using Domain.Persistance;

namespace Domain.AuthorRelated
{
    #region Dbo

    public class AuthorDbo : DboBase<Guid>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }

    #endregion

    #region Dto

    public class AuthorDto : DtoBase<Guid>
    {
        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }
    }

    public class AuthorDtoMapper : IDtoMapper<AuthorDbo, AuthorDto, Guid>
    {
        public AuthorDto? ToDto(AuthorDbo? value)
        {
            return value == null ? null : new AuthorDto
            {
                FirstName = value.FirstName,
                LastName = value.LastName,
                Id = value.UniqueId
            };
        }

        public void ToExistingDto(AuthorDbo value, ref AuthorDto existingDto)
        {
            existingDto.FirstName = value.FirstName;
            existingDto.LastName = value.LastName;
        }
    }

    #endregion
}
