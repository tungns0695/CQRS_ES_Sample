using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public interface IUnitOfWork<TContext> where TContext : DbContext
    {
        Task<int> SaveChangesAsync();
        IBaseRepository<TContext, T> Repository<T>() where T : BaseEntity;
    }
}