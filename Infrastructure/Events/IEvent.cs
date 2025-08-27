namespace Infrastructure.Events
{
    public interface IEvent
    {
        public string Id { get; set; }
        Guid AggregateId { get; set; }
        int Version { get; set; }
        DateTimeOffset OccuredOn { get; set; }
        long Position { get; set; }
    }
}