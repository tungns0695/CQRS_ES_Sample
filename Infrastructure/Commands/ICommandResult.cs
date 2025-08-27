namespace Infrastructure.Commands
{
    public interface ICommandResult<T>
    {
        T? Data { get; set; }
    }
}