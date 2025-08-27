using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public interface IReadRepository<TContext, T> where TContext : DbContext where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<T>> ListAllAsync();
    }
}
