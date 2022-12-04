using HotChocolate.AspNetCore.Authorization;

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
        WellKnownRoles.Service,
        WellKnownRoles.Root,
    })]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<UserRole> GetUserRoles(DataContext db) =>
        db.UserRoles;

    [Authorize(Roles = new[] {
        WellKnownRoles.Admin,
        WellKnownRoles.Service,
        WellKnownRoles.Root,
    })]
    [UseFirstOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<UserRole> GetFirstUserRole(DataContext db) =>
        db.UserRoles;

    [Authorize(Roles = new[] {
        WellKnownRoles.Admin,
        WellKnownRoles.Service
    })]
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<UserRole> GetUniqueUserRole(DataContext db) =>
        db.UserRoles;
}

public partial class Mutation
{
    [Authorize(Roles = new[] {
        WellKnownRoles.Admin,
        WellKnownRoles.Service,
        WellKnownRoles.Root,
    })]
    public async Task<UserRole> CreateUserRoleAsync(CreateUserRoleInput input, DataContext db)
    {
        _logger.Information("Creating UserRole...");
        _logger.Verbose("{@Input}", input);

        var userRole = await db.UserRoles.AddAsync(new()
        {
            UserID = input.UserID,
            RoleID = input.RoleID,
            Context = input.Context,
        });
        await db.SaveChangesAsync();

        _logger.Information("UserRole created!");
        return userRole.Entity;
    }
}