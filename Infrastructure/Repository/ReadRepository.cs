using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class ReadRepository<TContext, T> : IReadRepository<TContext, T> where TContext : DbContext where T : BaseEntity
    {
        private readonly TContext _dbContext;

        public ReadRepository(TContext dbContext, bool disableTracking = true)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            if (disableTracking)
            {
                _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }
        }

        public async virtual Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }
    }
}
