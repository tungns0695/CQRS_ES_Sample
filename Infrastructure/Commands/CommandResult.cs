namespace Infrastructure.Commands
{
    public class CommandResult<T> : ICommandResult<T>
    {
        public T? Data { get; set; }
    }
}