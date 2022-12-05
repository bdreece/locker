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
    public Me GetMe([Service] IHttpContextAccessor httpContextAccessor)
    {
        var ctx = httpContextAccessor.HttpContext;
        return new(
            ctx?.User.GetID() ?? string.Empty,
            ctx?.User.GetContext() ?? string.Empty,
            ctx?.User.GetName() ?? string.Empty,
            ctx?.User.GetRoles() ?? Enumerable.Empty<string>()
        );
    }
}