/**
 * locker - A multi-tenant GraphQL authentication & authorization server
 * Copyright (C) 2022 Brian Reece

 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
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