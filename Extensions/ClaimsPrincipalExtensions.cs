using System.Security.Claims;

namespace Locker;

public static class ClaimsPrincipalExtensions
{
    public static string? GetID(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.NameIdentifier);

    public static string? GetName(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Name);

    public static string? GetEmail(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Email);

    public static string? GetPhone(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.MobilePhone);

    public static string? GetContext(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.GroupSid);

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal) =>
        principal
            .FindAll(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);
}