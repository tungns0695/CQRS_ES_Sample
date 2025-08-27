using Infrastructure.Aggregate;

namespace Infrastructure.Snapshotting
{
    public interface ISnapshotRepository
    {
        Task<AggregateRoot> GetAsync(Guid aggregateId, CancellationToken ct = default);
        Task SaveAsync(AggregateRoot snapshot, CancellationToken ct = default);
    }
}