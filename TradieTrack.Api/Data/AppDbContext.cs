using Microsoft.EntityFrameworkCore;
using TradieTrack.Api.Models;

namespace TradieTrack.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Organization>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Plan).HasDefaultValue("Free");
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
        });

        b.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Email).IsRequired().HasMaxLength(320);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.FullName).HasMaxLength(200);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("now()");
            e.HasOne(x => x.Organization)
             .WithMany()
             .HasForeignKey(x => x.OrganizationId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Customer>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Email).HasMaxLength(320);
            e.Property(x => x.Phone).HasMaxLength(40);
            e.Property(x => x.Address).HasMaxLength(500);
            e.HasIndex(x => new { x.OrganizationId, x.Email }).HasFilter("\"Email\" IS NOT NULL");
            e.HasIndex(x => new { x.OrganizationId, x.Phone }).HasFilter("\"Phone\" IS NOT NULL");
            e.HasOne(x => x.Organization)
             .WithMany() // ✅ do not point to Users
             .HasForeignKey(x => x.OrganizationId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
