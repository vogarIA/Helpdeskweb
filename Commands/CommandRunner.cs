namespace HelpDeskWeb.Commands
{
    public static class CommandRunner
    {
        public static async Task<bool> TryRunAsync(string[] args, IServiceProvider services)
        {
            if (args.Length == 0) return false;

            var commands = new List<ICommand>
        {
            new SeedRolesCommand(),
            new SeedAdminCommand()
            // agregás nuevos comandos acá a futuro
        };

            var commandName = args[0];
            var command = commands.FirstOrDefault(c => c.Name == commandName);

            if (command == null)
            {
                Console.WriteLine($"Comando no reconocido: {commandName}");
                Console.WriteLine("Comandos disponibles: " + string.Join(", ", commands.Select(c => c.Name)));
                return true; // se consumió el arg aunque no se reconoció el comando
            }

            await command.ExecuteAsync(services, args.Skip(1).ToArray());
            return true;
        }
    }
}
