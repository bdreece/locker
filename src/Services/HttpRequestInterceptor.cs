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