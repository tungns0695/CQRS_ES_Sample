using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository
{
    public class BaseRepository<TContext, T> : ReadRepository<TContext, T>, IBaseRepository<TContext, T> where TContext : DbContext where T : BaseEntity
    {
        private readonly TContext _dbContext;

        public BaseRepository(TContext dbContext) : base(dbContext, false)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }
    }
}
