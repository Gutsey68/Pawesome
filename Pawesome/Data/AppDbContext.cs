using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pawesome.Models;
using Pawesome.Models.Entities;
using Pawesome.Models.enums;
using Pawesome.Models.Enums;

namespace Pawesome.Data;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<AnimalType> AnimalTypes { get; set; } = null!;
    public DbSet<Pet> Pets { get; set; } = null!;
    public DbSet<Address> Addresses { get; set; } = null!;
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<PasswordReset> PasswordResets { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
    public DbSet<Advert> Adverts { get; set; } = null!;
    public DbSet<PetAdvert> PetAdverts { get; set; } = null!;
    public DbSet<AnimalTypeAdvert> AnimalTypeAdverts { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Address)
            .WithMany()
            .HasForeignKey(u => u.AddressId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Pets)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Adverts)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Pet>()
            .HasOne(p => p.AnimalType)
            .WithMany()
            .HasForeignKey(p => p.AnimalTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PetAdvert>()
            .HasKey(pa => new { pa.PetId, pa.AdvertId });
        
        modelBuilder.Entity<PetAdvert>()
            .HasOne(pa => pa.Pet)
            .WithMany(p => p.PetAdverts)
            .HasForeignKey(pa => pa.PetId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<PetAdvert>()
            .HasOne(pa => pa.Advert)
            .WithMany(a => a.PetAdverts)
            .HasForeignKey(pa => pa.AdvertId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AnimalTypeAdvert>()
            .HasKey(ata => new { ata.AnimalTypeId, ata.AdvertId });
        
        modelBuilder.Entity<AnimalTypeAdvert>()
            .HasOne(ata => ata.AnimalType)
            .WithMany(at => at.AnimalTypeAdverts)
            .HasForeignKey(ata => ata.AnimalTypeId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<AnimalTypeAdvert>()
            .HasOne(ata => ata.Advert)
            .WithMany(a => a.AnimalTypeAdverts)
            .HasForeignKey(ata => ata.AdvertId)
            .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Booking)
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasKey(r => new { r.UserId, r.BookingId });

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Booking)
            .WithMany()
            .HasForeignKey(r => r.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Advert)
            .WithMany(a => a.Bookings)
            .HasForeignKey(b => b.AdvertId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.BookerUser)
            .WithMany()
            .HasForeignKey(b => b.BookerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Booking>()
            .Property(b => b.Status)
            .HasDefaultValue(BookingStatus.PendingConfirmation);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Status)
            .HasDefaultValue(PaymentStatus.Pending);

        modelBuilder.Entity<Advert>()
            .Property(a => a.Status)
            .HasDefaultValue(AdvertStatus.Pending);
    }
}