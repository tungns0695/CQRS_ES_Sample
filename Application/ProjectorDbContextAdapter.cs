using Infrastructure.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application
{
    public class ProjectorDbContextAdapter : IProjectorDbContext
    {
        private readonly TaxSystemDbContext _dbContext;

        public ProjectorDbContextAdapter(TaxSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DbSet<ProjectorCheckpoint> ProjectorCheckpoints => _dbContext.ProjectorCheckpoints;
        public DbSet<AppliedEvent> AppliedEvents => _dbContext.AppliedEvents;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }
    }
}

