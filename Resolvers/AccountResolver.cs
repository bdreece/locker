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
    public IQueryable<Account> GetAccounts(
        DataContext db,
        [GlobalState(tenantKey)] string? tenantID,
        [GlobalState(WellKnownRoles.Admin)] bool isAdmin
    ) =>
        isAdmin ?
            db.Accounts.Where(a => a.TenantID == tenantID)
            : db.Accounts;

    [Authorize(Roles = new[] {
        WellKnownRoles.Admin,
        WellKnownRoles.Root,
    })]
    [UseFirstOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Account> GetFirstAccount(
        DataContext db,
        [GlobalState(tenantKey)] string? tenantID,
        [GlobalState(WellKnownRoles.Admin)] bool isAdmin
    ) =>
        isAdmin ?
            db.Accounts.Where(a => a.TenantID == tenantID)
            : db.Accounts;

    [Authorize(Roles = new[] {
        WellKnownRoles.Admin,
        WellKnownRoles.Root
    })]
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Account> GetUniqueAccount(
        DataContext db,
        [GlobalState(tenantKey)] string? tenantID,
        [GlobalState(WellKnownRoles.Admin)] bool isAdmin
    ) =>
        isAdmin ?
            db.Accounts.Where(a => a.TenantID == tenantID)
            : db.Accounts;
}

public partial class Mutation
{
    [Authorize(Roles = new[] {
        WellKnownRoles.Admin,
        WellKnownRoles.Root,
    })]
    public async Task<Account> CreateAccountAsync(
        CreateAccountInput input,
        DataContext db,
        [GlobalState(WellKnownRoles.Admin)] bool isAdmin,
        [GlobalState(WellKnownRoles.Root)] bool isRoot,
        [GlobalState(tenantKey)] string? tenantID
    )
    {
        _logger.Information("Creating Account...");
        _logger.Verbose("{@Input}", input);

        if (isRoot && input.TenantID is null)
            throw new MissingOneOfException(nameof(input.TenantID));

        if (isAdmin && (
            tenantID is null || (
                input.TenantID is not null && input.TenantID != tenantID)))
            throw new UnauthorizedException();

        input.TenantID ??= tenantID;

        var account = await db.Accounts.AddAsync(new()
        {
            UserID = input.UserID,
            RoleID = input.RoleID,
            TenantID = input.TenantID!,
        });
        await db.SaveChangesAsync();

        _logger.Information("Account created!");
        return account.Entity;
    }

    [Authorize(Roles = new[] { WellKnownRoles.Root })]
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