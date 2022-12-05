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

    public async Task<SecurityToken> BuildAccessTokenAsync(IPrincipal principal, string context, TimeSpan? validFor = default)
    {
        var claims = Enumerable.Empty<Claim>();
        if (principal is User user)
            claims = await GetUserClaimsAsync(user, context);
        else if (principal is Service service)
            claims = GetServiceClaims(service);

        return BuildToken(principal, context, claims, validFor ?? TimeSpan.FromHours(2));
    }

    public SecurityToken BuildRefreshToken(IPrincipal principal, string context) =>
        BuildToken(principal, context, Enumerable.Empty<Claim>(), TimeSpan.FromDays(365));

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

    private SecurityToken BuildToken(IPrincipal principal, string context, IEnumerable<Claim> claims, TimeSpan validFor)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims.Concat(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, principal.ID),
                new(ClaimTypes.Name, principal.Name),
                new(ClaimTypes.Hash, principal.SecurityStamp),
            })),
            Expires = DateTime.UtcNow.Add(validFor),
            Audience = context,
            Issuer = _options.Issuer,
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
        };

        return _tokenHandler.CreateToken(tokenDescriptor);
    }

    private async Task<IEnumerable<Claim>> GetUserClaimsAsync(User user, string context)
    {
        var roles = await _db.UserRoles
            .Include(userRole => userRole.Role)
            .Where(userRole => userRole.UserID == user.ID)
            .Where(userRole => userRole.Context == context || userRole.Context == "root")
            .Select(userRole => userRole.Role!.Name)
            .ToArrayAsync();

        return roles
            .Select(role => new Claim(ClaimTypes.Role, role))
            .Concat(new Claim[]
            {
                new(ClaimTypes.Actor, WellKnownActors.User),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new(ClaimTypes.MobilePhone, user.Phone ?? string.Empty),
                new(ClaimTypes.GroupSid, context)
            });
    }

    private Claim[] GetServiceClaims(Service service) => new Claim[]
    {
        new(ClaimTypes.Actor, WellKnownActors.Service),
        new(ClaimTypes.Role, WellKnownRoles.Service),
        new(ClaimTypes.GroupSid, service.Context)
    };
}