using Microsoft.EntityFrameworkCore;
using ByteSymmetryTest.Models;

namespace ByteSymmetryTest.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.RowVersion).IsRowVersion();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(e => e.Total).HasPrecision(18, 2);
            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = adminId,
            FullName = "Admin",
            Email = "admin@gmail.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Yasar@2005!"),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        });

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 45999.00m,
                Stock = 50,
                CreatedAt = DateTime.UtcNow,
                RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
            },
            new Product
            {
                Id = 2,
                Name = "Mouse",
                Price = 599.00m,
                Stock = 200,
                CreatedAt = DateTime.UtcNow,
                RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
            },
            new Product
            {
                Id = 3,
                Name = "Keyboard",
                Price = 1499.00m,
                Stock = 150,
                CreatedAt = DateTime.UtcNow,
                RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
            }
        );
    }
}
