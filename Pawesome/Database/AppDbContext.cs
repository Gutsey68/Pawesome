using Microsoft.EntityFrameworkCore;
using Pawesome.Models;

public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the AppDbContext with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by this context.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<AnimalType> AnimalTypes { get; set; }
    public DbSet<Advert> Adverts { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<PasswordReset> PasswordResets { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PetAdvert> PetAdverts { get; set; }

    /// <summary>
    /// Configures the database model and entity relationships when creating the model.
    /// Overrides the base OnModelCreating method from DbContext.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PetAdvert>()
            .HasKey(pa => new { pa.PetId, pa.AdvertId });

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
    }
}