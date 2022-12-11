using System.Reflection;
using System.Runtime.CompilerServices;
using HotChocolate.Types.Descriptors;
using Microsoft.EntityFrameworkCore;

using Locker.Services;
using static Locker.Models.WellKnownGlobalStateKeys;

namespace Locker.Middlewares;

public class UseTenantAuthenticationAttribute : ObjectFieldDescriptorAttribute
{
    public UseTenantAuthenticationAttribute([CallerLineNumber] int order = 0) =>
        Order = order;

    public override void OnConfigure(
        IDescriptorContext _context,
        IObjectFieldDescriptor descriptor,
        MemberInfo _member
    )
    {
        descriptor.Use(next => async ctx =>
        {
            var dbContextFactory = ctx.Services.GetRequiredService<IDbContextFactory<DataContext>>();
            var tenantID = ctx.GetGlobalValue<string>(TenantID);
            var tenantKey = ctx.GetGlobalValue<string>(TenantKey);
            var isRoot = ctx.GetGlobalValue<bool>(IsRoot);

            if (!isRoot)
            {
                await using var db = await dbContextFactory.CreateDbContextAsync();

                var tenant = await db.Tenants.SingleOrDefaultAsync(t => t.ID == tenantID);
                if (tenant is null || tenant.ApiKey != tenantKey)
                    throw new UnauthenticatedException();
            }

            await next(ctx);
        });
    }
}