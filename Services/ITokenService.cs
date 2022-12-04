using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Locker.Models.Entities;

namespace Locker.Services;

public interface ITokenService
{
    Task<SecurityToken> BuildAccessTokenAsync(User user, string context);
    SecurityToken BuildRefreshToken(User user, string context);

    string Encode(SecurityToken token);
    (ClaimsPrincipal principal, SecurityToken token) Decode(string token);
}