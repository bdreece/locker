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
        DataContext db,
        [Service] IHttpContextAccessor httpContextAccessor,
        [Service] ITokenService tokenService,
        [GlobalState(tenantKey)] string tenantID
    )
    {
        _logger.Information("Parsing refresh token...");
        var ctx = httpContextAccessor.HttpContext;
        var refreshToken = ctx?.Request.Cookies
            .FirstOrDefault(c => c.Key == $"locker_refresh_{tenantID}").Value;

        if (refreshToken is null)
            throw new MissingTokenException(TokenType.RefreshToken);

        var (_principal, token) = tokenService.Decode(refreshToken);

        var id = _principal.GetID();
        var tokenTenantID = _principal.GetTenantID();
        var securityStamp = _principal.GetSecurityStamp();

        if (id is null || tokenTenantID is null || tokenTenantID != tenantID)
            throw new UnauthenticatedException();

        _logger.Information("Querying user...");
        var user = await db.Users
            .SingleOrDefaultAsync(u => u.ID == id)
            .ConfigureAwait(false);

        if (user is null)
            throw new EntityNotFoundException(typeof(User));

        if (user.SecurityStamp != securityStamp)
            throw new UnauthenticatedException();

        _logger.Information("Creating access token");
        var accessToken = await tokenService.BuildAccessTokenAsync(user, tokenTenantID);
        var accessJwt = tokenService.Encode(accessToken);

        return new(accessJwt, accessToken.ValidTo);
    }
}