using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public interface IBaseRepository<TContext, T> : IReadRepository<TContext, T> where TContext : DbContext where T : BaseEntity
    {
        Task<T> AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
