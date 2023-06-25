using System;
using System.ComponentModel.DataAnnotations;
using Domain.AuthorRelated;
using Domain.Dtos;
using Domain.Persistance;

namespace Domain.BookRelated
{
    #region Dbo

    public class BookDbo : DboBase<Guid>
    {
        public required string Title { get; set; }
        public AuthorDbo? Author { get; set; }
        public required int Year { get; set; }
        public required string Publisher { get; set; }
        public int? DbComputedColumn { get; set; }
        public DateTime? DbDefaultValueColumn { get; set; }
    }

    #endregion

    #region Dto

    public class BookDto : DtoBase<Guid>
    {
        [Required]
        public required string Title { get; set; }
        public AuthorDto? Author { get; set; }
        [Required]
        public required int Year { get; set; }
        [Required]
        public required string Publisher { get; set; }
        public int? DbComputedColumn { get; set; }
        public DateTime? DbDefaultValueColumn { get; set; }
    }

    public class BookDtoMapper : IDtoMapper<BookDbo, BookDto, Guid>
    {
        private readonly IDtoMapper<AuthorDbo, AuthorDto, Guid> authorDtoMapper;

        public BookDtoMapper(IDtoMapper<AuthorDbo, AuthorDto, Guid> authorDtoMapper)
        {
            this.authorDtoMapper = authorDtoMapper;
        }

        public BookDto? ToDto(BookDbo? value)
        {
            return value == null
                ? null
                : new BookDto
                {
                    Title = value.Title,
                    Author = authorDtoMapper.ToDto(value.Author),
                    Year = value.Year,
                    Publisher = value.Publisher,
                    Id = value.UniqueId,
                    DbComputedColumn = value.DbComputedColumn,
                    DbDefaultValueColumn = value.DbDefaultValueColumn
                };
        }
        public void ToExistingDto(BookDbo value, ref BookDto existingDto)
        {
            existingDto.Title = value.Title;
            existingDto.Author = authorDtoMapper.ToDto(value.Author);
            existingDto.Year = value.Year;
            existingDto.Publisher = value.Publisher;
            existingDto.DbComputedColumn = value.DbComputedColumn;
            existingDto.DbDefaultValueColumn = value.DbDefaultValueColumn;
        }
    }

    #endregion
}
