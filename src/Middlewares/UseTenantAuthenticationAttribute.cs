/**
 * locker - A multi-tenant GraphQL authentication & authorization server
 * Copyright (C) 2022 Brian Reece

 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
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
    ) => descriptor.Use(next => async ctx =>
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

            ctx.SetGlobalValue<bool>(TenantAuthState, true);
        }

        await next(ctx);
    });
}