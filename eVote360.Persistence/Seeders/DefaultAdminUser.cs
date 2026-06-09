using eVote360.Core.Domain.Entities;
using eVote360.Core.Domain.Enums;
using eVote360.Infrastructure.Persistence.Contexts;
using Microsoft.Extensions.DependencyInjection;

namespace eVote360.Infrastructure.Persistence.Seeders
{
    public static class DefaultAdminUser
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (!context.Usuarios.Any(u => u.NombreUsuario == "admin"))
            {
                context.Usuarios.Add(new Usuario
                {
                    Nombre = "Super",
                    Apellido = "Administrador",
                    CorreoElectronico = "admin@evote360.com",
                    NombreUsuario = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
                    Rol = Roles.Administrador.ToString(),
                    Estado = true
                });

                await context.SaveChangesAsync();
            }
        }
    }
}