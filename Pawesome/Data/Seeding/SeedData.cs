using Pawesome.Models;

namespace Pawesome.Data.Seeding
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider
                .GetRequiredService<AppDbContext>();

            context.Database.EnsureCreated();

            SeedRoles(context);
            SeedCountries(context);
            SeedCities(context);
            SeedAddresses(context);
            SeedUsers(context);
            SeedAnimalTypes(context);
            SeedPets(context);
            SeedAdverts(context);
            SeedMessages(context);
            SeedReviews(context);
            SeedNotifications(context);
            SeedReports(context);
            SeedPasswordResets(context);
            SeedPayments(context); 

            context.SaveChanges();
        }

        private static void SeedRoles(AppDbContext context)
        {
            if (context.Roles.Any()) return;

            context.Roles.AddRange(
                new Role { Name = "Admin", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role { Name = "User", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role { Name = "Moderator", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            
            context.SaveChanges();
        }

        private static void SeedCountries(AppDbContext context)
        {
            if (context.Countries.Any()) return;

            context.Countries.AddRange(
                new Country { Name = "France", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Country { Name = "Belgique", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Country { Name = "Suisse", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            
            context.SaveChanges();
        }

        private static void SeedCities(AppDbContext context)
        {
            if (context.Cities.Any()) return;

            var france = context.Countries.Single(c => c.Name == "France");
            var belgique = context.Countries.Single(c => c.Name == "Belgique");
            var suisse = context.Countries.Single(c => c.Name == "Suisse");

            context.Cities.AddRange(
                new City { Name = "Paris", PostalCode = "75000", CountryId = france.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Lyon", PostalCode = "69000", CountryId = france.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Bruxelles", PostalCode = "1000", CountryId = belgique.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new City { Name = "Genève", PostalCode = "1200", CountryId = suisse.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            
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
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Address
                {
                    StreetAddress = "456 Avenue Victor Hugo",
                    AdditionalInfo = "Apt 42",
                    CityId = paris.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            
            context.SaveChanges();
        }

        private static void SeedUsers(AppDbContext context)
        {
            if (context.Users.Any()) return;

            var adminRole = context.Roles.Single(r => r.Name == "Admin");
            var userRole = context.Roles.Single(r => r.Name == "User");
            var moderatorRole = context.Roles.Single(r => r.Name == "Moderator");
            var addresses = context.Addresses.ToList();

            context.Users.AddRange(
                new User
                {
                    LastName = "Admin",
                    FirstName = "Super",
                    Email = "admin@pawesome.com",
                    Password = "hashed_password",
                    Bio = "Administrateur de la plateforme Pawesome",
                    Photo = "admin.jpg",
                    Rating = 5.0f,
                    Role = adminRole,
                    Status = "Active",
                    IsVerified = true,
                    BalanceAccount = 0m,
                    OnboardingStep = 5,
                    IsOnboardingCompleted = true,
                    CompletedProfile = 100,
                    RoleId = adminRole.Id,
                    AddressId = addresses[0].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    LastName = "Dupont",
                    FirstName = "Jean",
                    Email = "user1@example.com",
                    Password = "hashed_password",
                    Bio = "Amoureux des animaux depuis toujours",
                    Photo = "jean.jpg",
                    Rating = 4.5f,
                    Role = userRole,
                    Status = "Active",
                    IsVerified = true,
                    BalanceAccount = 100m,
                    OnboardingStep = 5,
                    IsOnboardingCompleted = true,
                    CompletedProfile = 90,
                    RoleId = userRole.Id,
                    AddressId = addresses[1].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    LastName = "Martin",
                    FirstName = "Sophie",
                    Email = "sophie@example.com",
                    Password = "hashed_password",
                    Bio = "Passionnée par les chiens et chats",
                    Photo = "sophie.jpg",
                    Rating = 4.8f,
                    Role = userRole,
                    Status = "Active",
                    IsVerified = true,
                    BalanceAccount = 150m,
                    OnboardingStep = 5,
                    IsOnboardingCompleted = true,
                    CompletedProfile = 95,
                    RoleId = userRole.Id,
                    AddressId = addresses[0].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    LastName = "Dubois",
                    FirstName = "Michel",
                    Email = "moderator@example.com",
                    Password = "hashed_password",
                    Bio = "Modérateur expérimenté",
                    Photo = "michel.jpg",
                    Rating = 4.9f,
                    Role = moderatorRole,
                    Status = "Active",
                    IsVerified = true,
                    BalanceAccount = 50m,
                    OnboardingStep = 5,
                    IsOnboardingCompleted = true,
                    CompletedProfile = 100,
                    RoleId = moderatorRole.Id,
                    AddressId = addresses[1].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            
            context.SaveChanges();
        }

        private static void SeedAnimalTypes(AppDbContext context)
        {
            if (context.AnimalTypes.Any()) return;

            context.AnimalTypes.AddRange(
                new AnimalType { Name = "Chien", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new AnimalType { Name = "Chat", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new AnimalType { Name = "Oiseau", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new AnimalType { Name = "Rongeur", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            
            context.SaveChanges();
        }

        private static void SeedPets(AppDbContext context)
        {
            if (context.Pets.Any()) return;

            var users = context.Users.ToList();
            var chien = context.AnimalTypes.First(a => a.Name == "Chien");
            var chat = context.AnimalTypes.First(a => a.Name == "Chat");
            var oiseau = context.AnimalTypes.First(a => a.Name == "Oiseau");
            var rongeur = context.AnimalTypes.First(a => a.Name == "Rongeur");

            context.Pets.AddRange(
                new Pet
                {
                    Name = "Rex",
                    Breed = "Berger Allemand",
                    Age = 3,
                    Photo = "rex.jpg",
                    Info = "Un gentil berger allemand",
                    UserId = users[1].Id,
                    AnimalTypeId = chien.Id,
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
                    UserId = users[1].Id,
                    AnimalTypeId = chat.Id,
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
                    UserId = users[2].Id,
                    AnimalTypeId = oiseau.Id,
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
                    UserId = users[2].Id,
                    AnimalTypeId = rongeur.Id,
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
                    UserId = users[3].Id,
                    AnimalTypeId = chien.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            
            context.SaveChanges();
        }

        private static void SeedAdverts(AppDbContext context)
        {
            if (context.Adverts.Any()) return;

            var users = context.Users.ToList();

            var advert1 = new Advert
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(3),
                Status = "pending",
                Amount = 50.0m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            var advert2 = new Advert
            {
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(6),
                Status = "pending",
                Amount = 15.0m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var advert3 = new Advert
            {
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(4),
                Status = "approved",
                Amount = 30.0m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var advert4 = new Advert
            {
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(10),
                Status = "approved",
                Amount = 75.0m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            context.Adverts.AddRange(advert1, advert2, advert3, advert4);
            context.SaveChanges();

            var pets = context.Pets.ToList();

            context.PetAdverts.AddRange(
                new PetAdvert { PetId = pets[0].Id, AdvertId = advert1.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new PetAdvert { PetId = pets[1].Id, AdvertId = advert2.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new PetAdvert { PetId = pets[2].Id, AdvertId = advert3.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new PetAdvert { PetId = pets[3].Id, AdvertId = advert4.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new PetAdvert { PetId = pets[4].Id, AdvertId = advert3.Id, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
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
                    ReceiverId = users[1].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Message
                {
                    Content = "Merci pour votre message. Quand seriez-vous disponible?",
                    Status = "read",
                    SenderId = users[1].Id,
                    ReceiverId = users[0].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Message
                {
                    Content = "Je peux passer demain vers 14h si cela vous convient.",
                    Status = "unread",
                    SenderId = users[0].Id,
                    ReceiverId = users[1].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Message
                {
                    Content = "Bonjour, est-ce que votre chat s'entend bien avec les enfants?",
                    Status = "unread",
                    SenderId = users[2].Id,
                    ReceiverId = users[1].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Message
                {
                    Content = "Pourriez-vous me donner plus d'informations sur les soins spécifiques pour votre hamster?",
                    Status = "read",
                    SenderId = users[1].Id,
                    ReceiverId = users[2].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            
            context.SaveChanges();
        }

        private static void SeedReviews(AppDbContext context)
        {
            if (context.Reviews.Any()) return;

            var users = context.Users.ToList();
            var adverts = context.Adverts.ToList();
            
            if (users.Count < 1 || adverts.Count < 1) return;

            context.Reviews.AddRange(
                new Review
                {
                    Rate = 5,
                    Comment = "Excellent service, je recommande !",
                    UserId = users[1].Id,
                    AdvertId = adverts[0].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Review
                {
                    Rate = 4,
                    Comment = "Très bonne expérience, animal adorable",
                    UserId = users[2].Id,
                    AdvertId = adverts[1].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Review
                {
                    Rate = 5,
                    Comment = "Tout s'est parfaitement déroulé, merci !",
                    UserId = users[0].Id,
                    AdvertId = adverts[2].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            
            context.SaveChanges();
        }
        
        private static void SeedNotifications(AppDbContext context)
        {
            if (context.Notifications.Any()) return;

            var users = context.Users.ToList();
            if (users.Count < 2) return;

            context.Notifications.AddRange(
                new Notification
                {
                    Comment = "Quelqu'un a répondu à votre annonce",
                    UserId = users[0].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Notification
                {
                    Comment = "Votre annonce a été approuvée",
                    UserId = users[1].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Notification
                {
                    Comment = "Vous avez reçu un nouveau message",
                    UserId = users[2].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Notification
                {
                    Comment = "Votre réservation a été confirmée",
                    UserId = users[1].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Notification
                {
                    Comment = "Un utilisateur a laissé un avis sur votre service",
                    UserId = users[3].Id,
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
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Report
                {
                    Comment = "Cet utilisateur ne s'est pas présenté au rendez-vous",
                    UserId = users[1].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Report
                {
                    Comment = "Les informations sur l'animal ne correspondent pas à la réalité",
                    UserId = users[2].Id,
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
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new PasswordReset
                {
                    Token = Guid.NewGuid().ToString(),
                    IsValid = false,
                    ExpiresAt = DateTime.UtcNow.AddHours(-24),
                    UserId = users[1].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                }
            );

            context.SaveChanges();
        }

        private static void SeedPayments(AppDbContext context)
        {
            if (context.Payments.Any()) return;

            var users = context.Users.ToList();
            var adverts = context.Adverts.ToList();
            if (users.Count < 1 || adverts.Count < 1) return;

            context.Payments.AddRange(
                new Payment
                {
                    Amount = 50.0m,
                    Status = "completed",
                    UserId = users[0].Id,
                    AdvertId = adverts[0].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Payment
                {
                    Amount = 15.0m,
                    Status = "pending",
                    UserId = users[1].Id,
                    AdvertId = adverts[1].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Payment
                {
                    Amount = 30.0m,
                    Status = "completed",
                    UserId = users[2].Id,
                    AdvertId = adverts[2].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Payment
                {
                    Amount = 75.0m,
                    Status = "cancelled",
                    UserId = users[3].Id,
                    AdvertId = adverts[3].Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            context.SaveChanges();
        }
    }
}