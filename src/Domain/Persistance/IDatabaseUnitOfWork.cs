using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Domain.Persistance
{
    public interface IDatabaseUnitOfWork
    {
        /// <summary>
        /// Nesting of transaction scopes is supported with default arguments.
        /// </summary>
        /// <remarks>
        /// Use <see cref="TransactionScope.Complete"/> to commit the transaction when called on root scope,
        /// or to complete a nested scope without committing the transaction yet.
        /// </remarks>
        TransactionScope CreateTransactionScope(TransactionScopeOption transactionScopeOption = TransactionScopeOption.Required, TimeSpan? timeout = null);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        void ResetChanges();
    }
}
