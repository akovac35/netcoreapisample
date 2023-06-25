using System;
using Domain.Persistance;

namespace Domain.BookRelated
{
    public interface IBookRepository : IRepository<BookDbo, Guid>
    {
    }
}
