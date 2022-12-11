using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Locker.Models.Entities;

namespace Locker.Services;

public interface ITokenService : IAsyncDisposable
{
    Task<SecurityToken> BuildAccessTokenAsync(User principal, string tenantID, TimeSpan? validFor = default);
    SecurityToken BuildRefreshToken(User principal, string tenantID);

    string Encode(SecurityToken token);
    (ClaimsPrincipal principal, SecurityToken token) Decode(string token);
}