using System.Security.Claims;
using Locker.Models;

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

    public static string? GetTenantID(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.GroupSid);

    public static string? GetSecurityStamp(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Hash);

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal) =>
        principal
            .FindAll(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);

    public static bool IsUser(this ClaimsPrincipal principal) =>
        principal
            .FindAll(c => c.Type == ClaimTypes.Role)
            .All(c => c.Value == WellKnownRoles.User || (
                c.Value != WellKnownRoles.Admin
                && c.Value != WellKnownRoles.Root
            ));

    public static string GetAccessLevel(this ClaimsPrincipal principal)
    {
        var roles = principal.GetRoles();
        var (hasUser, hasAdmin, hasRoot) = roles.Aggregate(
            new ValueTuple<bool, bool, bool>(),
            (flags, role) =>
            {
                if (role == WellKnownRoles.User)
                    flags.Item1 = true;
                else if (role == WellKnownRoles.Admin)
                    flags.Item2 = true;
                else if (role == WellKnownRoles.Root)
                    flags.Item3 = true;
                return flags;
            });

        if (hasUser && !(hasAdmin || hasRoot))
            return WellKnownRoles.User;
        else if (hasAdmin && !hasRoot)
            return WellKnownRoles.Admin;
        else if (hasRoot)
            return WellKnownRoles.Root;

        throw new UnauthorizedException();
    }
}