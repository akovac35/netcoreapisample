using System;
using System.Linq;
using Domain.BookRelated;
using Domain.Persistance;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.BookRelated
{
    public class BookRepository : RepositoryBase<BookDbo, Guid, SampleDbContext>, IBookRepository
    {
        public BookRepository(SampleDbContext dbContext, IDatabaseUnitOfWork databaseUnitOfWork) : base(dbContext, databaseUnitOfWork)
        {
        }

        protected override IQueryable<BookDbo> ConfigureQueryable()
        {
            return base.ConfigureQueryable().Include(q => q.Author);

        }
    }
}
