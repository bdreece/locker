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

using Locker.Models.Entities;
using Locker.Services;

using static Locker.Models.WellKnownGlobalStateKeys;

namespace Locker.Middlewares;

public class UseTenantFilteringAttribute : ObjectFieldDescriptorAttribute
{
    public UseTenantFilteringAttribute([CallerLineNumber] int order = 0) =>
        Order = order;

    public override void OnConfigure(
        IDescriptorContext _context,
        IObjectFieldDescriptor descriptor,
        MemberInfo _member
    ) => descriptor.Use(next => async ctx =>
    {
        var isRoot = ctx.GetGlobalValue<bool>(IsRoot);
        if (!isRoot)
        {
            var tenantID = ctx.GetGlobalValue<string>(TenantID);
            if (ctx.Result is IQueryable<Account> accounts)
            {
                ctx.Result = accounts.Where(a => a.TenantID == tenantID);
            }
            else if (ctx.Result is IQueryable<User> users)
            {
                ctx.Result = users
                    .Include(u => u.Accounts)
                    .Where(u => u.Accounts.Any(a => a.TenantID == tenantID));
            }
            else if (ctx.Result is IQueryable<Role> roles)
            {
                ctx.Result = roles.Where(r => r.TenantID == tenantID || r.TenantID == null);
            }
        }

        await next(ctx);
    });
}