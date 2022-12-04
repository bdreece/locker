using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

namespace Locker.Resolvers;

public record class RegisterPayload(
    [property: GraphQLType(typeof(IdType))]
    [property: GraphQLName("id")]
    string ID
);

public partial class Mutation
{
    [Error(typeof(MissingOneOfException))]
    [Error(typeof(EntityConflictException))]
    public async Task<RegisterPayload> RegisterAsync(
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

        var hasUser = await db.Users.Where(predicate).AnyAsync();
        if (hasUser)
            throw new EntityConflictException(typeof(User));

        var user = new User
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            Phone = input.Phone
        };

        user.Hash = hashService.HashPassword(user, input.Password);

        var entry = await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        return new(entry.Entity.ID);
    }
}