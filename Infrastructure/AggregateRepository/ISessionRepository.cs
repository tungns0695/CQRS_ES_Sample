using Infrastructure.Aggregate;

namespace Infrastructure.AggregateRepository
{
    public interface IAggregateRepository
    {
        Task AddAsync<T>(T aggregate) where T : AggregateRoot;

        Task<T> GetAsync<T>(Guid id, int? version = null) where T : AggregateRoot;

        Task<bool> ExistAsync(Guid id);

        Task CommitAsync();
    }
}