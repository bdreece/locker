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
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

using static Locker.Models.WellKnownGlobalStateKeys;

namespace Locker.Resolvers;

public partial class Mutation
{
    [Error(typeof(MissingOneOfException))]
    [Error(typeof(EntityConflictException))]
    public async Task<User> RegisterAsync(
        RegisterInput input,
        DataContext db,
        [Service] IPasswordHasher<User> hashService,
        [GlobalState(TenantKey)] string? tenantID
    )
    {
        Expression<Func<User, bool>>? predicate = default;
        if (input.Email is not null)
            predicate = u => u.Email == input.Email;
        else if (input.Phone is not null)
            predicate = u => u.Phone == input.Phone;
        else
            throw new MissingOneOfException("email", "phone");

        _logger.Information("Checking if user exists...");
        var hasUser = await db.Users.Where(predicate).AnyAsync();
        if (hasUser)
            throw new EntityConflictException(typeof(User));

        _logger.Information("Creating new user...");
        var roles = new Role[]
        {
            await db.Roles.SingleAsync(r => r.Name == "user"),
        };

        var tenant = await db.Tenants.SingleOrDefaultAsync(t => t.ID == tenantID);
        if (tenant is null)
            throw new EntityNotFoundException(typeof(Tenant));

        var accounts = roles
            .Select(role => new Account
            {
                Role = role,
                Tenant = tenant,
            })
            .ToList();

        var user = new User
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            Phone = input.Phone,
            Accounts = accounts,
        };
        _logger.Verbose("{@User}", user);
        user.UpdateHash(hashService.HashPassword(user, input.Password));

        _logger.Information("Persisting user to database...");
        var entry = await db.AddAsync(user);

        await db.SaveChangesAsync();

        _logger.Information("User registered!");
        return entry.Entity;
    }
}