using System;
using Domain.Persistance;

namespace Domain.ConsentRelated
{
    public interface IConsentRepository: IRepository<ConsentDbo, Guid>
    {
    }
}
