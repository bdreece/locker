using System.Security.Claims;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Locker.Models;
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
        _logger.Information("Parsing refresh token...");
        var ctx = httpContextAccessor.HttpContext;
        var refreshToken = ctx?.Request.Cookies
            .FirstOrDefault(c => c.Key == input.Context).Value;

        if (refreshToken is null)
            throw new MissingTokenException(TokenType.RefreshToken);

        var (_principal, token) = tokenService.Decode(refreshToken);

        var id = _principal.GetID();
        var context = _principal.GetContext();
        var securityStamp = _principal.GetSecurityStamp();

        if (id is null || context is null)
            throw new UnauthenticatedException();

        _logger.Information("Querying principal...");
        IPrincipal? principal = _principal.GetActor() switch
        {
            WellKnownActors.User => await db.Users
                .SingleOrDefaultAsync(u => u.ID == id)
                .ConfigureAwait(false),
            WellKnownActors.Service => await db.Services
                .SingleOrDefaultAsync(s => s.ID == id)
                .ConfigureAwait(false),
            _ => throw new ArgumentException("Invalid actor"),
        };

        if (principal is null)
            throw new EntityNotFoundException(typeof(IPrincipal));

        if (principal.SecurityStamp != securityStamp)
            throw new UnauthenticatedException();

        _logger.Information("Creating access token");
        var accessToken = await tokenService.BuildAccessTokenAsync(principal, context);
        var accessJwt = tokenService.Encode(accessToken);

        return new(accessJwt, accessToken.ValidTo);
    }
}