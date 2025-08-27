namespace Infrastructure.Exceptions
{
    public class AggregateVersionIncorrectException : Exception
    {
        public AggregateVersionIncorrectException()
            : base($"Aggregate version should not be less then or equal to 0")
        { }
    }
}
