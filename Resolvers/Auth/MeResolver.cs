using HotChocolate.AspNetCore.Authorization;

namespace Locker.Resolvers;

public record Me(
    [property: ID]
    [property: GraphQLName("id")]
    string ID,
    string Context,
    string Name,
    IEnumerable<string> Roles
);

public partial class Query
{
    [Authorize]
    public Me GetMe(
        [Service] IHttpContextAccessor httpContextAccessor,
        [GlobalState(tenantKey)] string? tenantID
    )
    {
        var ctx = httpContextAccessor.HttpContext;
        if (tenantID is null || tenantID != ctx?.User.GetTenantID())
            throw new UnauthorizedException();
        return new(
            ctx?.User.GetID() ?? string.Empty,
            ctx?.User.GetTenantID() ?? string.Empty,
            ctx?.User.GetName() ?? string.Empty,
            ctx?.User.GetRoles() ?? Enumerable.Empty<string>()
        );
    }
}