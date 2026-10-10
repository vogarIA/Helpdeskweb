namespace HelpDeskWeb.Commands
{
    public interface ICommand
    {
        string Name { get; }
        Task ExecuteAsync(IServiceProvider services, string[] args);
    }
}
