#nullable disable
using Microsoft.EntityFrameworkCore;

using Locker.Models.Entities;

namespace Locker.Services;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>()
            .HasAlternateKey(u => u.Email);

        builder.Entity<User>()
            .HasAlternateKey(u => u.Phone);

        builder.Entity<Role>()
            .HasAlternateKey(r => r.Name);

        builder.Entity<UserRole>()
            .HasIndex(ur => ur.Context);
    }

    public DbSet<User> Users { get; init; }
    public DbSet<UserRole> UserRoles { get; init; }
    public DbSet<Role> Roles { get; init; }
}