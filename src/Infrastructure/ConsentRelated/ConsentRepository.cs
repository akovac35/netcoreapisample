using System;
using Domain.ConsentRelated;
using Domain.Persistance;
using Infrastructure.Persistence;

namespace Infrastructure.ConsentRelated
{
    public class ConsentRepository : RepositoryBase<ConsentDbo, Guid, SampleDbContext>, IConsentRepository
    {
        public ConsentRepository(SampleDbContext dbContext, IDatabaseUnitOfWork databaseUnitOfWork) : base(dbContext, databaseUnitOfWork)
        {
        }
    }
}
