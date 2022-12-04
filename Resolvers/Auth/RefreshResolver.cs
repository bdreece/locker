using System.Security.Claims;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

namespace Locker.Resolvers;

public record class Refresh(string AccessToken, DateTime Expiration);

public partial class Query
{
    [Error(typeof(MissingTokenException))]
    [Error(typeof(UnauthenticatedException))]
    [Error(typeof(EntityNotFoundException))]
    public async Task<Refresh> RefreshAsync(
        RefreshInput input,
        DataContext db,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] ITokenService tokenService
    )
    {
        var ctx = httpContextAccessor.HttpContext;
        var refreshToken = ctx?.Request.Cookies
            .FirstOrDefault(c => c.Key == input.Context).Value;

        if (refreshToken is null)
            throw new MissingTokenException(TokenType.RefreshToken);

        var (principal, token) = tokenService.Decode(refreshToken);

        var id = principal.GetID();
        var context = principal.GetContext();

        if (id is null || context is null)
            throw new UnauthenticatedException();

        var user = await db.Users
            .Where(u => u.ID == id)
            .SingleOrDefaultAsync();

        if (user is null)
            throw new EntityNotFoundException(typeof(User));

        var accessToken = await tokenService.BuildAccessTokenAsync(user, context);
        var accessJwt = tokenService.Encode(accessToken);

        return new(accessJwt, accessToken.ValidTo);
    }
}