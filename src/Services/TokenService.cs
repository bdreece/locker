using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ILogger = Serilog.ILogger;

using Locker.Models;
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

    public async Task<SecurityToken> BuildAccessTokenAsync(User principal, string tenant, TimeSpan? validFor = default)
    {
        var claims = await GetUserClaimsAsync(principal, tenant);
        return BuildToken(principal, tenant, claims, validFor ?? TimeSpan.FromHours(2));
    }

    public SecurityToken BuildRefreshToken(User principal, string tenant) =>
        BuildToken(principal, tenant, Enumerable.Empty<Claim>(), TimeSpan.FromDays(365));

    public string Encode(SecurityToken token) =>
        _tokenHandler.WriteToken(token);

    public (ClaimsPrincipal principal, SecurityToken token) Decode(string token)
    {
        var principal = _tokenHandler.ValidateToken(token, new()
        {
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

    private async Task<IEnumerable<Claim>> GetUserClaimsAsync(User user, string tenantID)
    {
        var roles = await _db.Accounts
            .Include(acct => acct.Role)
            .Include(acct => acct.Tenant)
            .Where(acct => acct.UserID == user.ID)
            .Where(acct => acct.Tenant!.ID == tenantID || acct.Tenant.Name == "root")
            .Select(acct => acct.Role!.Name)
            .ToArrayAsync();

        return roles
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
}