namespace Infrastructure.Queries
{
    public class QueryResult<T> : IQueryResult<T>
    {
        public T? Data { get; set; }
    }
}