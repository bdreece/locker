using System.Reflection;
using System.Runtime.CompilerServices;
using HotChocolate.Types.Descriptors;

using static Locker.Models.WellKnownGlobalStateKeys;

namespace Locker.Middlewares;

public class UseTenantAuthorizationAttribute : ObjectFieldDescriptorAttribute
{
    public UseTenantAuthorizationAttribute([CallerLineNumber] int order = 0) =>
        Order = order;

    public override void OnConfigure(
        IDescriptorContext _context,
        IObjectFieldDescriptor descriptor,
        MemberInfo _member
    ) => descriptor.Use(next => async ctx =>
    {
        var isRoot = ctx.GetGlobalValue<bool>(IsRoot);
        var tenantAuthState = ctx.GetGlobalValue<bool>(TenantAuthState);

        if (!isRoot && !tenantAuthState)
            throw new UnauthorizedException();

        await next(ctx);
    });
}