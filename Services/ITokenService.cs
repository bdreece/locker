using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Locker.Models;

namespace Locker.Services;

public interface ITokenService : IAsyncDisposable
{
    Task<SecurityToken> BuildAccessTokenAsync(IPrincipal principal, string context, TimeSpan? validFor = default);
    SecurityToken BuildRefreshToken(IPrincipal principal, string context);

    string Encode(SecurityToken token);
    (ClaimsPrincipal principal, SecurityToken token) Decode(string token);
}