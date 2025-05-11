using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pawesome.Models;
using Pawesome.Models.Entities;

namespace Pawesome.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Pet> Pets { get; set; }
    public DbSet<AnimalType> AnimalTypes { get; set; }
    public DbSet<Advert> Adverts { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<PasswordReset> PasswordResets { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PetAdvert> PetAdverts { get; set; }
    public DbSet<AnimalTypeAdvert> AnimalTypeAdverts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PetAdvert>()
            .HasKey(pa => new { pa.PetId, pa.AdvertId });

        modelBuilder.Entity<AnimalTypeAdvert>()
            .HasKey(ata => new { ata.AnimalTypeId, ata.AdvertId });

        modelBuilder.Entity<AnimalTypeAdvert>()
            .HasOne(ata => ata.AnimalType)
            .WithMany()
            .HasForeignKey(ata => ata.AnimalTypeId);

        modelBuilder.Entity<AnimalTypeAdvert>()
            .HasOne(ata => ata.Advert)
            .WithMany()
            .HasForeignKey(ata => ata.AdvertId);

        modelBuilder.Entity<Review>()
            .HasKey(r => new { r.UserId, r.AdvertId });

        modelBuilder.Entity<Payment>()
            .HasKey(p => new { p.UserId, p.AdvertId });

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Receiver)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reports)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Advert>()
            .HasOne(a => a.User)
            .WithMany(u => u.Adverts)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Advert>()
            .HasOne(a => a.Address)
            .WithMany(ad => ad.Adverts)
            .HasForeignKey(a => a.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Address)
            .WithMany(a => a.Users)
            .HasForeignKey(u => u.AddressId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pet>()
            .HasOne(p => p.User)
            .WithMany(u => u.Pets)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Pet>()
            .HasOne(p => p.AnimalType)
            .WithMany()
            .HasForeignKey(p => p.AnimalTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PasswordReset>()
            .HasOne(pr => pr.User)
            .WithMany(u => u.PasswordResets)
            .HasForeignKey(pr => pr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<City>()
            .HasOne(c => c.Country)
            .WithMany()
            .HasForeignKey(c => c.CountryId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Address>()
            .HasOne(a => a.City)
            .WithMany()
            .HasForeignKey(a => a.CityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
