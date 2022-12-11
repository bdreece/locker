#nullable disable
using Microsoft.EntityFrameworkCore;

using Locker.Models.Entities;

namespace Locker.Services;

public class DataContext : DbContext
{
    public DataContext() { }
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder) =>
        builder.Entity<Role>()
            .HasAlternateKey(r => r.Name);

    public virtual DbSet<User> Users { get; init; }
    public virtual DbSet<Account> Accounts { get; init; }
    public virtual DbSet<Role> Roles { get; init; }
    public virtual DbSet<Tenant> Tenants { get; init; }
}