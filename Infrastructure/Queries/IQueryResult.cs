namespace Infrastructure.Queries
{
    public interface IQueryResult<T>
    {
        T? Data { get; set; }
    }
}