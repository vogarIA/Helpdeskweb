using Microsoft.AspNetCore.Identity;

namespace HelpDeskWeb.Commands
{
    public class SeedRolesCommand: ICommand
    {
        public string Name => "seed:roles";

        public async Task ExecuteAsync(IServiceProvider services, string[] args)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Administrador", "Usuario" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"Rol creado: {role}");
                }
                else
                {
                    Console.WriteLine($"Rol ya existe: {role}");
                }
            }

            Console.WriteLine("Seed de roles completado.");
        }
    }
}
