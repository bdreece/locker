using static Locker.Models.WellKnownRoles;

namespace Locker.Models;

public static class WellKnownGlobalStateKeys
{
    public const string TenantID = "TENANT_ID";
    public const string TenantKey = "TENANT_KEY";
    public const string TenantAuthState = "TENANT_AUTH_STATE";
    public const string IsUser = User;
    public const string IsAdmin = Admin;
    public const string IsRoot = Root;
}