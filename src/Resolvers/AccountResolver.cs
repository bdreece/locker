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

using Locker.Middlewares;
using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

using static Locker.Models.WellKnownRoles;
using static Locker.Models.WellKnownGlobalStateKeys;

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
    public IQueryable<Account> GetAccounts(DataContext db) =>
        db.Accounts;

    [Authorize(Roles = new[] { Admin, Root })]
    [UseTenantAuthentication]
    [UseTenantAuthorization]
    [UseTenantFiltering]
    [UseFirstOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Account> GetFirstAccount(DataContext db) =>
        db.Accounts;

    [Authorize(Roles = new[] { Admin, Root })]
    [UseTenantAuthentication]
    [UseTenantAuthorization]
    [UseTenantFiltering]
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Account> GetUniqueAccount(DataContext db) =>
        db.Accounts;
}

public partial class Mutation
{
    [Authorize(Roles = new[] { Admin, Root })]
    public async Task<Account> CreateAccountAsync(
        CreateAccountInput input,
        DataContext db,
        [GlobalState(IsAdmin)] bool isAdmin,
        [GlobalState(IsRoot)] bool isRoot,
        [GlobalState(TenantID)] string? tenantID
    )
    {
        _logger.Information("Creating Account...");
        _logger.Verbose("{@Input}", input);

        if (isRoot && input.TenantID is null)
            throw new MissingOneOfException(nameof(input.TenantID));

        if (isAdmin && (input.TenantID is not null && input.TenantID != tenantID))
            throw new UnauthorizedException();

        tenantID ??= input.TenantID;

        var account = await db.Accounts.AddAsync(new()
        {
            UserID = input.UserID,
            RoleID = input.RoleID,
            TenantID = tenantID!,
        });
        await db.SaveChangesAsync();

        _logger.Information("Account created!");
        return account.Entity;
    }

    [Error(typeof(EntityNotFoundException))]
    [Authorize(Roles = new[] { Root })]
    public async Task<Account> DeleteAccountAsync([ID] string id, DataContext db)
    {
        _logger.Information("Querying Account...");
        var account = await db.Accounts.SingleOrDefaultAsync(ur => ur.ID == id);
        if (account is null)
            throw new EntityNotFoundException(typeof(Account));

        _logger.Information("Deleting Account...");
        db.Accounts.Remove(account);
        await db.SaveChangesAsync();

        _logger.Information("Account deleted!");
        return account;
    }
}