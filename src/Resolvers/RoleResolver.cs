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
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using Locker.Models;
using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

using UseFilteringAttribute = HotChocolate.Data.UseFilteringAttribute;
using UseSortingAttribute = HotChocolate.Data.UseSortingAttribute;

namespace Locker.Resolvers;

public partial class Query
{
    [Authorize(Roles = new[] {
        WellKnownRoles.Admin,
        WellKnownRoles.Root,
    })]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Role> GetRoles(DataContext db) =>
        db.Roles;


    [Authorize(Roles = new[] {
        WellKnownRoles.Admin,
        WellKnownRoles.Root,
    })]
    [UseFirstOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Role> GetFirstRole(DataContext db) =>
        db.Roles;

    [Authorize(Roles = new[] {
        WellKnownRoles.Admin,
        WellKnownRoles.Root,
    })]
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Role> GetUniqueRole(DataContext db) =>
        db.Roles;
}

public partial class Mutation
{
    [Authorize(Roles = new[] { WellKnownRoles.Root })]
    public async Task<Role> CreateRoleAsync(
        CreateRoleInput input,
        DataContext db
    )
    {
        _logger.Information("Creating role...");
        _logger.Verbose("{@Input}", input);

        var role = await db.Roles.AddAsync(new()
        {
            Name = input.Name
        });
        await db.SaveChangesAsync();

        _logger.Information("Role created!");
        return role.Entity;
    }

    [Authorize(Roles = new[] { WellKnownRoles.Root })]
    public async Task<Role> DeleteRoleAsync([ID] string id, DataContext db)
    {
        _logger.Information("Querying role");
        var role = await db.Roles.SingleOrDefaultAsync(r => r.ID == id);
        if (role is null)
            throw new EntityNotFoundException(typeof(Role));

        _logger.Information("Deleting role...");
        db.Roles.Remove(role);
        await db.SaveChangesAsync();

        _logger.Information("Role deleted!");
        return role;
    }
}