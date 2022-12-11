#nullable disable
using Microsoft.EntityFrameworkCore;

using Locker.Models.Entities;

namespace Locker.Services;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) =>
        builder.Entity<Role>()
            .HasAlternateKey(r => r.Name);

    public DbSet<User> Users { get; init; }
    public DbSet<Account> Accounts { get; init; }
    public DbSet<Role> Roles { get; init; }
    public DbSet<Tenant> Tenants { get; init; }
}