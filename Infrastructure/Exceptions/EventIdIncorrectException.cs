namespace Infrastructure.Exceptions
{
    public class EventIdIncorrectException : Exception
    {
        public EventIdIncorrectException( Guid id, Guid aggregateId)
            : base($"Event {id} has a different Id from it's aggregates Id ({aggregateId})")
        { }
    }
}
