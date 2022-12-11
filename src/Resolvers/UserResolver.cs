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
using System.Security.Claims;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using Locker.Middlewares;
using Locker.Models;
using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

using static Locker.Models.WellKnownGlobalStateKeys;
using static Locker.Models.WellKnownRoles;

using UseFilteringAttribute = HotChocolate.Data.UseFilteringAttribute;
using UseSortingAttribute = HotChocolate.Data.UseSortingAttribute;

namespace Locker.Resolvers;

public partial class Query
{
    [Authorize(Roles = new[] { Admin, Root })]
    [UseTenantAuthentication]
    [UseTenantAuthorization]
    [UseTenantFiltering]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUsers(DataContext db) =>
        db.Users;

    [Authorize(Roles = new[] { Admin, Root })]
    [UseTenantAuthentication]
    [UseTenantAuthorization]
    [UseTenantFiltering]
    [UseFirstOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetFirstUser(DataContext db) =>
        db.Users;

    [Authorize(Roles = new[] { Admin, Root })]
    [UseTenantAuthentication]
    [UseTenantAuthorization]
    [UseTenantFiltering]
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUniqueUser(DataContext db) =>
        db.Users;
}


public partial class Mutation
{
    [Error(typeof(EntityNotFoundException))]
    [Authorize(Roles = new[] { WellKnownRoles.User, Admin, Root })]
    public async Task<User> UpdateUserAsync(
        [ID] string id,
        UpdateUserInput input,
        DataContext db,
        ClaimsPrincipal principal,
        [GlobalState(IsUser)] bool isUser,
        [GlobalState(IsAdmin)] bool isAdmin,
        [GlobalState(IsRoot)] bool isRoot
    )
    {
        _logger.Information("Authorizing request...");
        var isUpdatingSelf = principal.GetID() == id;

        // Users can only update themselves
        if (!isAdmin && !isRoot && !isUpdatingSelf)
            throw new UnauthorizedException();

        _logger.Information("Updating user {ID}...", id);
        _logger.Verbose("{@Input}", input);
        var user = await db.Users
            .Where(u => u.ID == id)
            .SingleOrDefaultAsync();

        if (user is null)
        {
            _logger.Warning("User not found!");
            throw new EntityNotFoundException(typeof(User));
        }

        user.Update(input);
        db.Users.Update(user);
        await db.SaveChangesAsync();

        _logger.Information("User updated!");
        return user;
    }

    [Authorize(Roles = new[] { Root })]
    public async Task<User> DeleteUserAsync([ID] string id, DataContext db)
    {
        _logger.Information("Querying user...");
        var user = await db.Users.SingleOrDefaultAsync(u => u.ID == id);
        if (user is null)
            throw new EntityNotFoundException(typeof(User));

        _logger.Information("Deleting user...");
        db.Users.Remove(user);
        await db.SaveChangesAsync();

        _logger.Information("User deleted!");
        return user;
    }
}