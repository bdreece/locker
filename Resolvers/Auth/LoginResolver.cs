using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

namespace Locker.Resolvers;

public record class Login(string AccessToken, DateTime Expiration);

public partial class Mutation
{
    [Error(typeof(MissingOneOfException))]
    [Error(typeof(BadCredentialsException))]
    public async Task<Login> LoginAsync(
        LoginInput input,
        DataContext db,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] IPasswordHasher<User> hashService,
        [Service] ITokenService tokenService
    )
    {
        var ctx = httpContextAccessor.HttpContext;

        Expression<Func<User, bool>>? predicate = default;
        if (input.Email is not null)
            predicate = u => u.Email == input.Email;
        else if (input.Phone is not null)
            predicate = u => u.Phone == input.Phone;
        else
            throw new MissingOneOfException("email", "phone");

        _logger.Information("Querying user from database...");
        var user = await db.Users
            .Where(predicate)
            .SingleOrDefaultAsync();

        if (user is null)
            throw new BadCredentialsException();

        _logger.Information("Verifying user password...");
        var result = hashService.VerifyHashedPassword(user, user.Hash, input.Password);
        if (result != PasswordVerificationResult.Success)
            throw new BadCredentialsException();

        _logger.Information("Creating access and refresh tokens...");
        var accessToken = await tokenService.BuildAccessTokenAsync(user, input.Context);
        var refreshToken = tokenService.BuildRefreshToken(user, input.Context);
        var accessJwt = tokenService.Encode(accessToken);
        var refreshJwt = tokenService.Encode(refreshToken);

        ctx?.Response.Headers.AddRefreshTokenCookie(refreshJwt, input.Context);

        _logger.Information("User logged in!");
        return new(accessJwt, accessToken.ValidTo);
    }
}