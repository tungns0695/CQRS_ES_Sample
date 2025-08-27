using Infrastructure.Events;

namespace Infrastructure.Events
{
    public interface IEventProjector
    {
        string ProjectorName { get; }
        Task<long> GetLastProcessedPositionAsync();
        Task ProcessEventAsync(IEvent @event, long position);
        Task UpdateCheckpointAsync(long position);
        Task<bool> HasEventBeenProcessedAsync(string eventId);
        Task MarkEventAsProcessedAsync(string eventId);
    }
}

