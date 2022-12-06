using System.Security.Claims;
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
    [Authorize(Roles = new[]
    {
        WellKnownRoles.Admin,
        WellKnownRoles.Root,
    })]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUsers(
        DataContext db,
        [GlobalState(WellKnownRoles.Admin)] bool isAdmin,
        [GlobalState(tenantKey)] string? tenantID
    ) =>
        isAdmin ?
            db.Users
                .Include(u => u.Accounts)
                .Where(u => u.Accounts.Any(a => a.TenantID == tenantID))
            : db.Users;

    [Authorize(Roles = new[]
    {
        WellKnownRoles.Admin,
        WellKnownRoles.Root,
    })]
    [UseFirstOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetFirstUser(
        DataContext db,
        [GlobalState(WellKnownRoles.Admin)] bool isAdmin,
        [GlobalState(tenantKey)] string? tenantID
    ) =>
        isAdmin ?
            db.Users
                .Include(u => u.Accounts)
                .Where(u => u.Accounts.Any(a => a.TenantID == tenantID))
            : db.Users;

    [Authorize(Roles = new[]
    {
        WellKnownRoles.Admin,
        WellKnownRoles.Root,
    })]
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetUniqueUser(
        DataContext db,
        [GlobalState(WellKnownRoles.Admin)] bool isAdmin,
        [GlobalState(tenantKey)] string? tenantID
    ) =>
        isAdmin ?
            db.Users
                .Include(u => u.Accounts)
                .Where(u => u.Accounts.Any(a => a.TenantID == tenantID))
            : db.Users;
}


public partial class Mutation
{
    [Error(typeof(EntityNotFoundException))]
    [Authorize(Roles = new[]
    {
        WellKnownRoles.User,
        WellKnownRoles.Admin,
        WellKnownRoles.Root,
    })]
    public async Task<User> UpdateUserAsync(
        [ID] string id,
        UpdateUserInput input,
        DataContext db,
        ClaimsPrincipal principal,
        [GlobalState(WellKnownRoles.User)] bool isUser,
        [GlobalState(WellKnownRoles.Admin)] bool isAdmin,
        [GlobalState(WellKnownRoles.Root)] bool isRoot
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

    [Authorize(Roles = new[] { WellKnownRoles.Root })]
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