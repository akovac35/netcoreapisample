using System.Threading;
using System.Threading.Tasks;
using Domain.Persistance;
using MediatR;

namespace Infrastructure.Behaviors
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class, IRequest<TResponse>
    {
        private readonly IDatabaseUnitOfWork databaseUnitOfWork;

        public TransactionBehavior(IDatabaseUnitOfWork databaseUnitOfWork)
        {
            this.databaseUnitOfWork = databaseUnitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // TODO HIGH rather use interfaces
            // Queries do not need transactions
            if (request.GetType().Name.ToUpperInvariant().EndsWith("QUERY", System.StringComparison.InvariantCulture))
            {
                var result = await next();
                return result;
            }
            else
            {
                using var transactionScope = databaseUnitOfWork.CreateTransactionScope();
                var result = await next();
                transactionScope.Complete();
                return result;
            }
        }
    }
}
