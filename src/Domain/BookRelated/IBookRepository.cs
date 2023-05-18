using System;
using Domain.Persistance;

namespace Domain.BookRelated
{
    public interface IBookRepository : IRepository<Book, BookDbo, Guid>
    {
    }
}
