using System;
using System.Threading.Tasks;
using Domain.Persistance;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ITransactionScopeExtensions
    {
        public static async Task AsDatabaseUnitOfWork<TRepository, TDatabaseUnitOfWork>(this IServiceScopeFactory serviceScopeFactory, Func<TRepository, Task> dbWork)
            where TRepository : class
            where TDatabaseUnitOfWork : class, IDatabaseUnitOfWork
        {
            using var scope = serviceScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<TRepository>();
            var databaseUnitOfWork = scope.ServiceProvider.GetRequiredService<TDatabaseUnitOfWork>();
            using var transactionScope = databaseUnitOfWork.CreateTransactionScope();

            await dbWork(repository);

            transactionScope.Complete();
        }
    }
}
