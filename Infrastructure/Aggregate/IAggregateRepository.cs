namespace Infrastructure.Aggregate
{
    public interface IAggregateRepository<T> where T : IAggregateRoot
    {
        Task SaveAsync(T aggregate);

        Task<T> GetAsync(Guid aggregateId, int? expectedVersion = null);

        Task<bool> ExistAsync(Guid aggregateId);
    }
}
