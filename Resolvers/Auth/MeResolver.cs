using HotChocolate.AspNetCore.Authorization;

namespace Locker.Resolvers;

public record MePayload(
    [property: GraphQLType(typeof(IdType))]
    [property: GraphQLName("id")]
    string ID,
    string Context,
    string Name,
    string? Email,
    string? Phone,
    IEnumerable<string> Roles
);

public partial class Query
{
    [Authorize]
    public MePayload GetMe([Service] IHttpContextAccessor httpContextAccessor)
    {
        var ctx = httpContextAccessor.HttpContext;
        return new(
            ctx?.User.GetID() ?? string.Empty,
            ctx?.User.GetContext() ?? string.Empty,
            ctx?.User.GetName() ?? string.Empty,
            ctx?.User.GetEmail(),
            ctx?.User.GetPhone(),
            ctx?.User.GetRoles() ?? Enumerable.Empty<string>()
        );
    }
}