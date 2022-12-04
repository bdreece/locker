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

public class TokenService : ITokenService, IAsyncDisposable
{
    private readonly ILogger _logger = Log.Logger.ForContext<TokenService>();
    private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();
    private readonly SymmetricSecurityKey _key;
    private readonly LockerOptions _options;
    private readonly DataContext _db;

    private const string issuer = "https://locker.bdreece.dev";

    public TokenService(IDbContextFactory<DataContext> factory, IOptions<LockerOptions> options)
    {
        _options = options.Value;
        _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.SecretKey));
        _db = factory.CreateDbContext();
    }

    public ValueTask DisposeAsync() => _db.DisposeAsync();

    public async Task<SecurityToken> BuildAccessTokenAsync(User user, string context)
    {
        var roles = await GetUserRolesAsync(user, context);
        return BuildToken(user, context, roles, TimeSpan.FromHours(2));
    }

    public SecurityToken BuildRefreshToken(User user, string context) =>
        BuildToken(user, context, Enumerable.Empty<string>(), TimeSpan.FromDays(365));

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

    private Task<List<string>> GetUserRolesAsync(User user, string context) =>
        _db.UserRoles
            .Include(userRole => userRole.Role)
            .Where(userRole => userRole.UserID == user.ID)
            .Where(userRole => userRole.Context == context)
            .Select(userRole => userRole.Role!.Name)
            .ToListAsync();

    private SecurityToken BuildToken(User user, string context, IEnumerable<string> roles, TimeSpan validFor)
    {
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(roleClaims.Concat(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.ID),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Email, user.Email ?? String.Empty),
                new(ClaimTypes.MobilePhone, user.Phone ?? String.Empty),
                new(ClaimTypes.GroupSid, context)
            })),
            Expires = DateTime.UtcNow.Add(validFor),
            Audience = context,
            Issuer = issuer,
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
        };

        return _tokenHandler.CreateToken(tokenDescriptor);
    }
}