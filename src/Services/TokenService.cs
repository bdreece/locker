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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ILogger = Serilog.ILogger;

using Locker.Models.Entities;

namespace Locker.Services;

public class TokenService : ITokenService
{
    private readonly ILogger _logger = Log.Logger.ForContext<TokenService>();
    private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
    private readonly SymmetricSecurityKey _key;
    private readonly LockerOptions _options;
    private readonly DataContext _db;

    public TokenService(IDbContextFactory<DataContext> factory, IOptions<LockerOptions> options)
    {
        _options = options.Value;
        _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.SecretKey));
        _db = factory.CreateDbContext();
    }

    public ValueTask DisposeAsync() => _db.DisposeAsync();

    public async Task<SecurityToken> BuildAccessTokenAsync(User principal, string tenantID, TimeSpan? validFor = default)
    {
        var roles = await GetUserRolesAsync(principal, tenantID);
        var claims = GetUserClaims(principal, tenantID, roles);
        return BuildToken(principal, tenantID, claims, validFor ?? TimeSpan.FromHours(2));
    }

    public SecurityToken BuildRefreshToken(User principal, string tenantID) =>
        BuildToken(principal, tenantID, GetUserClaims(principal, tenantID, Enumerable.Empty<string>()), TimeSpan.FromDays(365));

    public string Encode(SecurityToken token) =>
        _tokenHandler.WriteToken(token);

    public (ClaimsPrincipal principal, SecurityToken token) Decode(string token)
    {
        var principal = _tokenHandler.ValidateToken(token, new()
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = _options.Issuer,
            IssuerSigningKey = _key,
        }, out var securityToken);
        return (principal, securityToken);
    }

    private SecurityToken BuildToken(User principal, string tenantID, IEnumerable<Claim> claims, TimeSpan validFor)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(validFor),
            Audience = tenantID,
            Issuer = _options.Issuer,
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
        };

        return _tokenHandler.CreateToken(tokenDescriptor);
    }

    private async Task<IEnumerable<string>> GetUserRolesAsync(User user, string tenantID) =>
        await _db.Accounts
            .Include(acct => acct.Role)
            .Include(acct => acct.Tenant)
            .Where(acct => acct.UserID == user.ID)
            .Where(acct => acct.Tenant!.ID == tenantID || acct.Tenant.Name == "root")
            .Select(acct => acct.Role!.Name)
            .ToArrayAsync();

    private IEnumerable<Claim> GetUserClaims(User user, string tenantID, IEnumerable<string> roles) =>
        roles
            .Select(role => new Claim(ClaimTypes.Role, role))
            .Concat(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.ID),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Hash, user.SecurityStamp),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.MobilePhone, user.Phone ?? string.Empty),
                new(ClaimTypes.GroupSid, tenantID)
            });

}