using Microsoft.AspNetCore.Identity;
using Pawesome.Models;
using Pawesome.Models.Entities;
using Pawesome.Models.Enums;

namespace Pawesome.Data.Seeding
{
    public static class SeedData
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            context.Database.EnsureCreated();

            await SeedRolesAsync(roleManager);
            SeedAnimalTypes(context);

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            if (await roleManager.RoleExistsAsync("Admin") && 
                await roleManager.RoleExistsAsync("User") && 
                await roleManager.RoleExistsAsync("Moderator"))
                return;

            await roleManager.CreateAsync(new IdentityRole<int>("Admin"));
            await roleManager.CreateAsync(new IdentityRole<int>("User"));
            await roleManager.CreateAsync(new IdentityRole<int>("Moderator"));
        }

        private static void SeedAnimalTypes(AppDbContext context)
        {
            if (context.AnimalTypes.Any()) return;

            context.AnimalTypes.AddRange(
                new AnimalType { Name = "Chien", Pets = new List<Pet>(), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new AnimalType { Name = "Chat", Pets = new List<Pet>(), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new AnimalType { Name = "Oiseau", Pets = new List<Pet>(), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new AnimalType { Name = "Rongeur", Pets = new List<Pet>(), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            
            context.SaveChanges();
        }
    }
}