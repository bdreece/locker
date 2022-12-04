using HotChocolate.AspNetCore.Authorization;
using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

using UseFilteringAttribute = HotChocolate.Data.UseFilteringAttribute;
using UseSortingAttribute = HotChocolate.Data.UseSortingAttribute;

namespace Locker.Resolvers;

public partial class Query
{
    [Authorize(Roles = new[] { "admin", "service" })]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Role> GetRoles(DataContext db) =>
        db.Roles;


    [Authorize(Roles = new[] { "admin", "service" })]
    [UseFirstOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Role> GetFirstRole(DataContext db) =>
        db.Roles;

    [Authorize(Roles = new[] { "admin", "service" })]
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Role> GetUniqueRole(DataContext db) =>
        db.Roles;
}

public partial class Mutation
{
    [Authorize(Roles = new[] { "admin", "service" })]
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
}