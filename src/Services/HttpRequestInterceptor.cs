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
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Locker.Models;

namespace Locker.Services;

public class HttpRequestInterceptor : DefaultHttpRequestInterceptor
{
    private const string TENANT_ID_HEADER = "X-Locker-TenantID";
    private const string TENANT_KEY_HEADER = "X-Locker-TenantKey";
    private const string TENANT_ID = "TENANT_ID";
    private const string TENANT_KEY = "TENANT_KEY";

    public override async ValueTask OnCreateAsync(
        HttpContext context,
        IRequestExecutor requestExecutor,
        IQueryRequestBuilder requestBuilder,
        CancellationToken cancellationToken
    )
    {
        var tenantID = context.Request.Headers
            .SingleOrDefault(h => h.Key == TENANT_ID_HEADER)
            .Value
            .SingleOrDefault();
        var tenantKey = context.Request.Headers
            .SingleOrDefault(h => h.Key == TENANT_KEY_HEADER)
            .Value
            .SingleOrDefault();

        var accessLevel = context.User.GetAccessLevel();

        requestBuilder.SetProperties(new()
        {
            { TENANT_ID, tenantID },
            { TENANT_KEY, tenantKey },
            { accessLevel, true }
        });

        await base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
    }
}