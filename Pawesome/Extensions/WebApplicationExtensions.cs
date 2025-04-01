using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pawesome.Data.Seeding;
using Pawesome.Models;

namespace Pawesome.Extensions;

public static class WebApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            
            await context.Database.MigrateAsync();
            
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
            string[] roleNames = { "Admin", "User", "Moderator" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    Console.WriteLine($"Rôle {roleName} créé avec succès");
                }
            }
            
            var userManager = services.GetRequiredService<UserManager<User>>();
            await SeedData.SeedAsync(context, userManager, roleManager);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Une erreur s'est produite lors de l'initialisation de la base de données.");
            throw;
        }
    }
}