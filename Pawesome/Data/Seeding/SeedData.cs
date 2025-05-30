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
            SeedCountries(context);
            SeedCities(context);
            SeedAddresses(context);
            await SeedUsersAsync(context, userManager);
            SeedAnimalTypes(context);
            SeedPets(context);
            SeedAdverts(context);
            SeedMessages(context);
            SeedReports(context);
            SeedPasswordResets(context);

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

        private static void SeedCountries(AppDbContext context)
        {
            if (context.Countries.Any()) return;

            context.Countries.AddRange(
                new Country { 
                    Name = "France", 
                    Cities = new List<City>(),
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new Country { 
                    Name = "Belgique", 
                    Cities = new List<City>(),
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new Country { 
                    Name = "Suisse", 
                    Cities = new List<City>(),
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                }
            );
            
            context.SaveChanges();
        }

        private static void SeedCities(AppDbContext context)
        {
            if (context.Cities.Any()) return;

            var countries = context.Countries.ToDictionary(c => c.Name);

            var cities = new[]
            {
                new City { 
                    Name = "Paris", 
                    PostalCode = "75000", 
                    Country = countries["France"],
                    Addresses = new List<Address>(),
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new City { 
                    Name = "Lyon", 
                    PostalCode = "69000", 
                    Country = countries["France"],
                    Addresses = new List<Address>(),
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new City { 
                    Name = "Bruxelles", 
                    PostalCode = "1000", 
                    Country = countries["Belgique"],
                    Addresses = new List<Address>(),
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new City { 
                    Name = "Genève", 
                    PostalCode = "1200", 
                    Country = countries["Suisse"],
                    Addresses = new List<Address>(),
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                }
            };

            context.Cities.AddRange(cities);
            context.SaveChanges();
        }

        private static void SeedAddresses(AppDbContext context)
        {
            if (context.Addresses.Any()) return;

            var paris = context.Cities.Single(c => c.Name == "Paris");
            
            context.Addresses.AddRange(
                new Address
                {
                    StreetAddress = "123 Rue de Paris",
                    CityId = paris.Id,
                    City = paris,
                    Users = new List<User>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Address
                {
                    StreetAddress = "456 Avenue Victor Hugo",
                    AdditionalInfo = "Apt 42",
                    CityId = paris.Id,
                    City = paris,
                    Users = new List<User>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            
            context.SaveChanges();
        }

        private static async Task SeedUsersAsync(AppDbContext context, UserManager<User> userManager)
        {
            if (userManager.Users.Any()) return;

            var addresses = context.Addresses.ToList();

            var admin = new User
            {
                LastName = "Admin",
                FirstName = "Brice",
                Email = "admin@pawesome.com",
                UserName = "admin@pawesome.com",
                Bio = "Administrateur de la plateforme Pawesome",
                Photo = "brice.jpg",
                Rating = 5.0f,
                Status = UserStatus.Active,
                IsVerified = true,
                BalanceAccount = 0m,
                OnboardingStep = 5,
                IsOnboardingCompleted = true,
                CompletedProfile = 100,
                AddressId = addresses[0].Id,
                Address = context.Addresses.First(),
                Pets = new List<Pet>(),
                Notifications = new List<Notification>(),
                Reports = new List<Report>(),
                PasswordResets = new List<PasswordReset>(),
                SentMessages = new List<Message>(),
                ReceivedMessages = new List<Message>(),
                Reviews = new List<Review>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");

            var user1 = new User
            {
                LastName = "Dupont",
                FirstName = "Jean",
                Email = "user1@example.com",
                UserName = "user1@example.com",
                Bio = "Amoureux des animaux depuis toujours",
                Photo = "images/persona/guys_2.jpg",
                Rating = 4.5f,
                Status = UserStatus.Active,
                IsVerified = true,
                BalanceAccount = 100m,
                OnboardingStep = 5,
                IsOnboardingCompleted = true,
                CompletedProfile = 90,
                AddressId = addresses[1].Id,
                Address = context.Addresses.First(),
                Pets = new List<Pet>(),
                Notifications = new List<Notification>(),
                Reports = new List<Report>(),
                PasswordResets = new List<PasswordReset>(),
                SentMessages = new List<Message>(),
                ReceivedMessages = new List<Message>(),
                Reviews = new List<Review>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(user1, "User1234!");
            await userManager.AddToRoleAsync(user1, "User");

            var user2 = new User
            {
                LastName = "Martin",
                FirstName = "Sophie",
                Email = "sophie@example.com",
                UserName = "sophie@example.com",
                Bio = "Passionnée par les chiens et chats",
                Photo = "images/persona/girl_1.jpg",
                Rating = 4.8f,
                Status = UserStatus.Active,
                IsVerified = true,
                BalanceAccount = 150m,
                OnboardingStep = 5,
                IsOnboardingCompleted = true,
                CompletedProfile = 95,
                AddressId = addresses[0].Id,
                Address = context.Addresses.First(),
                Pets = new List<Pet>(),
                Notifications = new List<Notification>(),
                Reports = new List<Report>(),
                PasswordResets = new List<PasswordReset>(),
                SentMessages = new List<Message>(),
                ReceivedMessages = new List<Message>(),
                Reviews = new List<Review>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(user2, "Sophie123!");
            await userManager.AddToRoleAsync(user2, "User");

            var moderator = new User
            {
                LastName = "Dubois",
                FirstName = "Michel",
                Email = "moderator@example.com",
                UserName = "moderator@example.com",
                Bio = "Modérateur expérimenté",
                Photo = "michel.jpg",
                Rating = 4.9f,
                Status = UserStatus.Active,
                IsVerified = true,
                BalanceAccount = 50m,
                OnboardingStep = 5,
                IsOnboardingCompleted = true,
                CompletedProfile = 100,
                AddressId = addresses[1].Id,
                Address = context.Addresses.First(),
                Pets = new List<Pet>(),
                Notifications = new List<Notification>(),
                Reports = new List<Report>(),
                PasswordResets = new List<PasswordReset>(),
                SentMessages = new List<Message>(),
                ReceivedMessages = new List<Message>(),
                Reviews = new List<Review>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(moderator, "Moderator123!");
            await userManager.AddToRoleAsync(moderator, "Moderator");
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

        private static void SeedPets(AppDbContext context)
        {
            if (context.Pets.Any()) return;

            var users = context.Users.ToList();
            var animalTypes = context.AnimalTypes.ToDictionary(at => at.Name);

            var pets = new[]
            {
                new Pet
                {
                    Name = "Rex",
                    Breed = "Berger Allemand",
                    Age = 3,
                    Photo = "rex.jpg",
                    Info = "Un gentil berger allemand",
                    UserId = users[0].Id,
                    User = users[0],
                    AnimalTypeId = animalTypes["Chien"].Id,
                    AnimalType = animalTypes["Chien"],
                    PetAdverts = new List<PetAdvert>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Pet
                {
                    Name = "Félix",
                    Breed = "Siamois",
                    Age = 2,
                    Photo = "felix.jpg",
                    Info = "Un chat très joueur",
                    UserId = users[0].Id,
                    User = users[0],
                    AnimalTypeId = animalTypes["Chat"].Id,
                    AnimalType = animalTypes["Chat"],
                    PetAdverts = new List<PetAdvert>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Pet
                {
                    Name = "Piou",
                    Breed = "Canari",
                    Age = 1,
                    Photo = "piou.jpg",
                    Info = "Chante dès le lever du soleil",
                    UserId = users[1].Id,
                    User = users[1],
                    AnimalTypeId = animalTypes["Oiseau"].Id,
                    AnimalType = animalTypes["Oiseau"],
                    PetAdverts = new List<PetAdvert>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Pet
                {
                    Name = "Noisette",
                    Breed = "Hamster doré",
                    Age = 1,
                    Photo = "noisette.jpg",
                    Info = "Très actif la nuit",
                    UserId = users[1].Id,
                    User = users[1],
                    AnimalTypeId = animalTypes["Rongeur"].Id,
                    AnimalType = animalTypes["Rongeur"],
                    PetAdverts = new List<PetAdvert>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Pet
                {
                    Name = "Luna",
                    Breed = "Labrador",
                    Age = 4,
                    Photo = "luna.jpg",
                    Info = "Très douce avec les enfants",
                    UserId = users[2].Id,
                    User = users[2],
                    AnimalTypeId = animalTypes["Chien"].Id,
                    AnimalType = animalTypes["Chien"],
                    PetAdverts = new List<PetAdvert>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            context.Pets.AddRange(pets);
            context.SaveChanges();
        }

        private static void SeedAdverts(AppDbContext context)
        {
            if (context.Adverts.Any()) return;

            var users = context.Users.ToList();

            var advert1 = new Advert
            {
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(9),
                Status = AdvertStatus.Pending,
                Amount = 50.0m,
                UserId = users[0].Id,
                PetAdverts = new List<PetAdvert>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AnimalTypeAdverts = new List<AnimalTypeAdvert>(),

            };
            
            var advert2 = new Advert
            {
                StartDate = DateTime.UtcNow.AddDays(10),
                EndDate = DateTime.UtcNow.AddDays(23),
                Status = AdvertStatus.Pending,
                Amount = 15.0m,
                UserId = users[0].Id,
                PetAdverts = new List<PetAdvert>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AnimalTypeAdverts = new List<AnimalTypeAdvert>(),
            };

            var advert3 = new Advert
            {
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(4),
                Status =  AdvertStatus.Active,
                Amount = 30.0m,
                UserId = users[1].Id,
                PetAdverts = new List<PetAdvert>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AnimalTypeAdverts = new List<AnimalTypeAdvert>(),
            };

            var advert4 = new Advert
            {
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(10),
                Status = AdvertStatus.Active,
                Amount = 75.0m,
                UserId = users[0].Id,
                PetAdverts = new List<PetAdvert>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                AnimalTypeAdverts = new List<AnimalTypeAdvert>(),
            };
            
            context.Adverts.AddRange(advert1, advert2, advert3, advert4);
            context.SaveChanges();

            var pets = context.Pets.ToList();

            context.PetAdverts.AddRange(
                new PetAdvert { 
                    PetId = pets[0].Id, 
                    Pet = pets[0],
                    AdvertId = advert1.Id, 
                    Advert = advert1,
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new PetAdvert { 
                    PetId = pets[1].Id, 
                    Pet = pets[1],
                    AdvertId = advert2.Id, 
                    Advert = advert2,
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new PetAdvert { 
                    PetId = pets[2].Id, 
                    Pet = pets[2],
                    AdvertId = advert3.Id, 
                    Advert = advert3,
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new PetAdvert { 
                    PetId = pets[3].Id, 
                    Pet = pets[3],
                    AdvertId = advert4.Id, 
                    Advert = advert4,
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new PetAdvert { 
                    PetId = pets[4].Id, 
                    Pet = pets[4],
                    AdvertId = advert3.Id, 
                    Advert = advert3,
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                }
            );
            
            context.SaveChanges();
        }

        private static void SeedMessages(AppDbContext context)
        {
            if (context.Messages.Any()) return;

            var users = context.Users.ToList();
            if (users.Count < 2) return;

            context.Messages.AddRange(
                new Message
                {
                    Content = "Bonjour, je suis intéressé par votre annonce",
                    Status = "unread",
                    SenderId = users[0].Id,
                    Sender = users[0],
                    ReceiverId = users[1].Id,
                    Receiver = users[1],
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Message
                {
                    Content = "Merci pour votre message. Quand seriez-vous disponible?",
                    Status = "read",
                    SenderId = users[1].Id,
                    Sender = users[1],
                    ReceiverId = users[0].Id,
                    Receiver = users[0],
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Message
                {
                    Content = "Je peux passer demain vers 14h si cela vous convient.",
                    Status = "unread",
                    SenderId = users[0].Id,
                    Sender = users[0],
                    ReceiverId = users[1].Id,
                    Receiver = users[1],
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Message
                {
                    Content = "Bonjour, est-ce que votre chat s'entend bien avec les enfants?",
                    Status = "unread",
                    SenderId = users[2].Id,
                    Sender = users[2],
                    ReceiverId = users[1].Id,
                    Receiver = users[1],
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Message
                {
                    Content = "Pourriez-vous me donner plus d'informations sur les soins spécifiques pour votre hamster?",
                    Status = "read",
                    SenderId = users[1].Id,
                    Sender = users[1],
                    ReceiverId = users[2].Id,
                    Receiver = users[2],
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            
            context.SaveChanges();
        }
        

        private static void SeedReports(AppDbContext context)
        {
            if (context.Reports.Any()) return;

            var users = context.Users.ToList();
            if (users.Count < 1) return;

            context.Reports.AddRange(
                new Report
                {
                    Comment = "Cette annonce contient des informations trompeuses",
                    UserId = users[0].Id,
                    User = users[0],
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Report
                {
                    Comment = "Cet utilisateur ne s'est pas présenté au rendez-vous",
                    UserId = users[1].Id,
                    User = users[1],
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Report
                {
                    Comment = "Les informations sur l'animal ne correspondent pas à la réalité",
                    UserId = users[2].Id,
                    User = users[2],
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            context.SaveChanges();
        }

        private static void SeedPasswordResets(AppDbContext context)
        {
            if (context.PasswordResets.Any()) return;

            var users = context.Users.ToList();
            if (!users.Any()) return;

            context.PasswordResets.AddRange(
                new PasswordReset
                {
                    Token = Guid.NewGuid().ToString(),
                    IsValid = true,
                    ExpiresAt = DateTime.UtcNow.AddDays(1),
                    UserId = users[0].Id,
                    User = users[0],
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new PasswordReset
                {
                    Token = Guid.NewGuid().ToString(),
                    IsValid = false,
                    ExpiresAt = DateTime.UtcNow.AddHours(-24),
                    UserId = users[1].Id,
                    User = users[1],
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                }
            );

            context.SaveChanges();
        }
    }
}