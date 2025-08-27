namespace Infrastructure.Exceptions
{
    public class EventsOutOfOrderException : Exception
    {
        public EventsOutOfOrderException(Guid id)
            : base($"EventStore gave events for aggregate {id} out of order")
        { }
    }
}
