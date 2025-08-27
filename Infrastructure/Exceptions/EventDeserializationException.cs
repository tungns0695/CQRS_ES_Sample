namespace Infrastructure.Exceptions
{
    public class EventDeserializationException : Exception
    {
        public EventDeserializationException(string message)
            : base(message)
        { }
    }
}
