using Microsoft.AspNetCore.Identity;
using System.Text;
using HelpDeskWeb.Models;

namespace HelpDeskWeb.Commands
{
    public class SeedAdminCommand: ICommand
    {
        public string Name => "seed:admin";

        public async Task ExecuteAsync(IServiceProvider services, string[] args)
        {
            var userManager = services.GetRequiredService<UserManager<Usser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Permite pasar --email como argumento, o pedirlo interactivo
            var email = GetArgValue(args, "--email");
            if (string.IsNullOrWhiteSpace(email))
            {
                Console.Write("Email del administrador: ");
                email = Console.ReadLine();
            }

            if (!await roleManager.RoleExistsAsync("Administrador"))
            {
                Console.WriteLine("El rol Administrador no existe. Corré 'seed:roles' primero.");
                return;
            }

            var existing = await userManager.FindByEmailAsync(email!);
            if (existing != null)
            {
                Console.WriteLine("Ya existe un usuario con ese email.");
                return;
            }

            Console.Write("Contraseña: ");
            var password = ReadPasswordMasked();

            var user = new Usser
            {
                UserName = email,
                Email = email,
                FullName = "Administrador",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Administrador");
                Console.WriteLine("Usuario administrador creado correctamente.");
            }
            else
            {
                foreach (var error in result.Errors)
                    Console.WriteLine($"Error: {error.Description}");
            }
        }

        private static string? GetArgValue(string[] args, string key)
        {
            var index = Array.IndexOf(args, key);
            return (index >= 0 && index + 1 < args.Length) ? args[index + 1] : null;
        }

        private static string ReadPasswordMasked()
        {
            var password = new StringBuilder();
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password.ToString();
        }
    }
}
