/**
 * locker - A multi-tenant GraphQL authentication & authorization server
 * Copyright (C) 2022 Brian Reece

 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using Microsoft.EntityFrameworkCore;
using Locker.Models.Entities;
using Locker.Services;

using static Locker.Models.WellKnownGlobalStateKeys;

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
        [GlobalState(TenantID)] string tenantID
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