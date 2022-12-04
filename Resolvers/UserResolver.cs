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
    public IQueryable<User> GetUsers(DataContext db) =>
        db.Users;

    [Authorize(Roles = new[] { "admin", "service" })]
    [UseFirstOrDefault]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> GetFirstUser(DataContext db) =>
        db.Users;

    [Authorize(Roles = new[] { "admin", "service" })]
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
    [Authorize(Roles = new[] { "admin", "service" })]
    public async Task<User> UpdateUserAsync(
        [GraphQLType(typeof(IdType))] string id,
        UpdateUserInput input,
        DataContext db
    )
    {
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
}