using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Locker.Models;

namespace Locker.Services;

public class HttpRequestInterceptor : DefaultHttpRequestInterceptor
{
    private const string TENANT_ID_HEADER = "X-Locker-TenantID";
    private const string TENANT_KEY_HEADER = "X-Locker-TenantKey";
    private const string TENANT = "tenant";

    private readonly DataContext _db;

    public HttpRequestInterceptor(DataContext db) =>
        _db = db;

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

        var tenant = await _db.Tenants.SingleOrDefaultAsync(t => t.ID == tenantID);
        if (tenant is not null && tenantKey != tenant.ApiKey)
            requestBuilder.SetProperty(TENANT, tenantID);

        var accessLevel = context.User.GetAccessLevel();
        requestBuilder.SetProperty(accessLevel, true);

        await base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
    }
}