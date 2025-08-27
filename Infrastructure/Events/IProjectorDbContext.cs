using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Events
{
    public interface IProjectorDbContext
    {
        DbSet<ProjectorCheckpoint> ProjectorCheckpoints { get; }
        DbSet<AppliedEvent> AppliedEvents { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    }
}

