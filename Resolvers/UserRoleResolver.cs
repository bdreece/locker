using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

using UseFilteringAttribute = HotChocolate.Data.UseFilteringAttribute;
using UseSortingAttribute = HotChocolate.Data.UseSortingAttribute;

namespace Locker.Resolvers;

public partial class Query
{
    [Authorize(Roles = new[] { "admin", "service" })]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<UserRole> GetUserRoles(DataContext db) =>
        db.UserRoles;

    [Authorize(Roles = new[] { "admin", "service" })]
    [UseFirstOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<UserRole> GetFirstUserRole(DataContext db) =>
        db.UserRoles;
    [Authorize(Roles = new[] { "admin", "service" })]
    [UseSingleOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<UserRole> GetUniqueUserRole(DataContext db) =>
        db.UserRoles;
}