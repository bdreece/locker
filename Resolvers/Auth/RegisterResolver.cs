using System.Linq.Expressions;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

namespace Locker.Resolvers;

public record class Register(
    [property: ID]
    [property: GraphQLName("id")]
    string ID
);

public partial class Mutation
{
    [Error(typeof(MissingOneOfException))]
    [Error(typeof(EntityConflictException))]
    public async Task<Register> RegisterAsync(
        RegisterInput input,
        DataContext db,
        [Service] IPasswordHasher<User> hashService
    )
    {

        Expression<Func<User, bool>>? predicate = default;
        if (input.Email is not null)
            predicate = u => u.Email == input.Email;
        else if (input.Phone is not null)
            predicate = u => u.Phone == input.Phone;
        else
            throw new MissingOneOfException("email", "phone");

        _logger.Information("Checking if user exists...");
        var hasUser = await db.Users.Where(predicate).AnyAsync();
        if (hasUser)
            throw new EntityConflictException(typeof(User));

        _logger.Information("Creating new user...");
        var roles = new Role[]
        {
            await db.Roles.SingleAsync(r => r.Name == "user"),
        };

        var userRoles = roles
            .Select(role => new UserRole
            {
                Role = role,
                Context = input.Context
            })
            .ToList();


        var user = new User
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            Phone = input.Phone,
            UserRoles = userRoles,
        };
        _logger.Verbose("{@User}", user);
        user.UpdateHash(hashService.HashPassword(user, input.Password));

        _logger.Information("Persisting user to database...");
        var entry = await db.AddAsync(user);

        await db.SaveChangesAsync();

        _logger.Information("User registered!");
        return new(entry.Entity.ID);
    }
}