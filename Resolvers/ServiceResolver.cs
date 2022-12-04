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
    public IQueryable<Service> GetServices(DataContext db) =>
        db.Services;

    [Authorize(Roles = new[] {
        WellKnownRoles.Admin,
        WellKnownRoles.Service,
        WellKnownRoles.Root,
    })]
    [UseFirstOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Service> GetFirstService(DataContext db) =>
        db.Services;

    [Authorize(Roles = new[] {
        WellKnownRoles.Admin,
        WellKnownRoles.Service,
        WellKnownRoles.Root,
    })]
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Service> GetUniqueService(DataContext db) =>
        db.Services;
}

public partial class Mutation
{
    [Authorize(Roles = new[] { WellKnownRoles.Root })]
    public async Task<Service> CreateServiceAsync(
        CreateServiceInput input,
        DataContext db
    )
    {
        var service = await db.Services.AddAsync(new()
        {
            Name = input.Name,
        });
        await db.SaveChangesAsync();
        return service.Entity;
    }
}