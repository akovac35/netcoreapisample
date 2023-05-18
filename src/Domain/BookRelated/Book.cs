using System;
using System.ComponentModel.DataAnnotations;
using Domain.Dtos;
using Domain.Persistance;

namespace Domain.BookRelated
{
    public class Book : IDboMappable<Guid>, IDtoMappable<Guid>
    {
        public Book(string title, string author, int year, string publisher, Guid? id = null)
        {
            Title = title;
            Author = author;
            Year = year;
            Publisher = publisher;

            Id = id ?? Guid.NewGuid();
        }

        public Guid Id { get; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Publisher { get; set; }

        public Guid DboUniqueId => Id;
        public int DboVersion { get; }

        public Guid DtoUniqueId => Id;
    }

    #region Dbo

    public class BookDbo : DboBase<Guid>
    {
        public required string Title { get; set; }
        public required string Author { get; set; }
        public required int Year { get; set; }
        public required string Publisher { get; set; }
    }

    public class BookDboMapper : IDboMapper<Book, BookDbo, Guid>
    {
        public Book? FromDbo(BookDbo? dbo)
        {
            return dbo == null ? null : new Book(dbo.Title, dbo.Author, dbo.Year, dbo.Publisher, dbo.UniqueId);
        }

        public BookDbo? ToDbo(Book? value)
        {
            return value == null
                ? null
                : new BookDbo
                {
                    Title = value.Title,
                    Author = value.Author,
                    Year = value.Year,
                    Publisher = value.Publisher,
                    RowVersion = value.DboVersion,
                    UniqueId = value.DboUniqueId
                };
        }
        public void ToExistingDbo(Book value, ref BookDbo existingDbo)
        {
            existingDbo.Title = value.Title;
            existingDbo.Author = value.Author;
            existingDbo.Year = value.Year;
            existingDbo.Publisher = value.Publisher;
        }
    }

    #endregion

    #region Dto

    public class BookDto : DtoBase<Guid>
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Author { get; set; }
        [Required]
        public required int Year { get; set; }
        [Required]
        public required string Publisher { get; set; }
    }

    public class BookDtoMapper : IDtoMapper<Book, BookDto, Guid>
    {
        public Book? FromDto(BookDto? dto)
        {
            return dto == null ? null : new Book(dto.Title, dto.Author, dto.Year, dto.Publisher, dto.Id);
        }

        public BookDto? ToDto(Book? value)
        {
            return value == null
                ? null
                : new BookDto
                {
                    Title = value.Title,
                    Author = value.Author,
                    Year = value.Year,
                    Publisher = value.Publisher,
                    Id = value.DtoUniqueId
                };
        }
        public void ToExistingDto(Book value, ref BookDto existingDto)
        {
            existingDto.Title = value.Title;
            existingDto.Author = value.Author;
            existingDto.Year = value.Year;
            existingDto.Publisher = value.Publisher;
        }

        public void ToExistingInstance(BookDto dto, ref Book existingInstance)
        {
            existingInstance.Title = dto.Title;
            existingInstance.Author = dto.Author;
            existingInstance.Year = dto.Year;
            existingInstance.Publisher = dto.Publisher;
        }
    }

    #endregion
}
