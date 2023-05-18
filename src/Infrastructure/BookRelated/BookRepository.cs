using System;
using Domain.BookRelated;
using Domain.Persistance;
using Infrastructure.Persistence;
using Microsoft.Extensions.Localization;

namespace Infrastructure.BookRelated
{
    public class BookRepository : RepositoryBase<Book, BookDbo, Guid, SampleDbContext>, IBookRepository
    {
        public BookRepository(IDboMapper<Book, BookDbo, Guid> dboMapper, SampleDbContext dbContext, IDatabaseUnitOfWork databaseUnitOfWork, IStringLocalizerFactory localizerFactory) : base(dboMapper, dbContext, databaseUnitOfWork, localizerFactory)
        {
        }
    }
}
