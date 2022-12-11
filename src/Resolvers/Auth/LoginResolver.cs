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
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

using static Locker.Models.WellKnownGlobalStateKeys;

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
        [Service] ITokenService tokenService,
        [GlobalState(TenantID)] string? tenantID
    )
    {
        if (tenantID is null)
            throw new UnauthorizedException();
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
        var accessToken = await tokenService.BuildAccessTokenAsync(user, tenantID);
        var refreshToken = tokenService.BuildRefreshToken(user, tenantID);
        var accessJwt = tokenService.Encode(accessToken);
        var refreshJwt = tokenService.Encode(refreshToken);

        ctx?.Response.Headers.AddRefreshTokenCookie(refreshJwt, tenantID);

        _logger.Information("User logged in!");
        return new(accessJwt, accessToken.ValidTo);
    }
}