using Microsoft.EntityFrameworkCore;

using Locker.Models.Entities;
using Locker.Models.Inputs;
using Locker.Services;

namespace Locker.Resolvers;

public record class AuthenticateService(
    string AccessToken,
    DateTime Expiration
);

public partial class Mutation
{
    [Error(typeof(EntityNotFoundException))]
    public async Task<AuthenticateService> AuthenticateServiceAsync(
        AuthenticateServiceInput input,
        DataContext db,
        [Service] ITokenService tokenService
    )
    {
        _logger.Information("Querying service...");
        var service = await db.Services
            .Where(s => s.Name == input.Name)
            .Where(s => s.ApiKey == input.ApiKey)
            .SingleOrDefaultAsync();

        if (service is null)
            throw new EntityNotFoundException(typeof(Service));

        _logger.Information("Creating service access token...");
        var accessToken = await tokenService.BuildAccessTokenAsync(service, service.Context);
        var accessJwt = tokenService.Encode(accessToken);

        _logger.Information("Service authenticated!");
        return new(accessJwt, accessToken.ValidTo);
    }
}