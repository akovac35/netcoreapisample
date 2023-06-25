using System;
using Domain.Persistance;

namespace Domain.AuthorRelated
{
    public interface IAuthorRepository : IRepository<AuthorDbo, Guid>
    {
    }
}
