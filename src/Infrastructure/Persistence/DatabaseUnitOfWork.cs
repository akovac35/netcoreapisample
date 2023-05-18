using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Domain.Persistance;

namespace Infrastructure.Persistence
{
    public class DatabaseUnitOfWork : IDatabaseUnitOfWork
    {
        private readonly SampleDbContext dbContext;

        public DatabaseUnitOfWork(SampleDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Resets unsaved changes.
        /// </summary>
        public void ResetChanges()
        {
            dbContext.ChangeTracker.Clear();
        }

        public TransactionScope CreateTransactionScope(TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TimeSpan? timeout = null)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            };

            if (timeout.HasValue)
            {
                transactionOptions.Timeout = timeout.Value;
            }

            return new TransactionScope(transactionScopeOption, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
        }
    }
}
