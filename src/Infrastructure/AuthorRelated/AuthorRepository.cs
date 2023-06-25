using System;
using Domain.AuthorRelated;
using Domain.Persistance;
using Infrastructure.Persistence;

namespace Infrastructure.AuthorRelated
{
    public class AuthorRepository : RepositoryBase<AuthorDbo, Guid, SampleDbContext>, IAuthorRepository
    {
        public AuthorRepository(SampleDbContext dbContext, IDatabaseUnitOfWork databaseUnitOfWork) : base(dbContext, databaseUnitOfWork)
        {
        }
    }
}
