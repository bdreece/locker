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
namespace Locker.Testing.Mocks;

public static class ServiceMock
{
    public static IServiceProvider Services { get; }
    public static RequestExecutorProxy Executor { get; }
    public static DataContextMock DataContextMock { get; } = new();

    static ServiceMock()
    {
        Services = new ServiceCollection()
            .AddHttpContextAccessor()
            .AddSingleton<DataContext>(DataContextMock.DataContext)
            .AddGraphQLServer()
            .RegisterDbContext<DataContext>()
            .AddHttpRequestInterceptor<HttpRequestInterceptor>()
            .AddGlobalObjectIdentification()
            .AddAuthorization()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddMutationConventions(applyToAllMutations: true)
            .AddQueryFieldToMutationPayloads()
            .Services
            .AddSingleton(
                sp => new RequestExecutorProxy(
                    sp.GetRequiredService<IRequestExecutorResolver>(),
                    Schema.DefaultName))
            .BuildServiceProvider();

        Executor = Services.GetRequiredService<RequestExecutorProxy>();
    }

    public static async Task<string> ExecuteRequestAsync(
        Action<IQueryRequestBuilder> configureRequest,
        CancellationToken cancellationToken = default
    )
    {
        await using var scope = Services.CreateAsyncScope();

        var requestBuilder = new QueryRequestBuilder();
        requestBuilder.SetServices(scope.ServiceProvider);
        configureRequest(requestBuilder);
        var request = requestBuilder.Create();

        using var result = await Executor.ExecuteAsync(request, cancellationToken);

        result.ExpectQueryResult();

        return await result.ToJsonAsync();
    }

    public static async IAsyncEnumerable<string> ExecuteRequestAsStreamAsync(
        Action<IQueryRequestBuilder> configureRequest,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await using var scope = Services.CreateAsyncScope();

        var requestBuilder = new QueryRequestBuilder();
        requestBuilder.SetServices(scope.ServiceProvider);
        configureRequest(requestBuilder);
        var request = requestBuilder.Create();

        using var result = await Executor.ExecuteAsync(request, cancellationToken);

        await foreach (var element in result.ExpectResponseStream().ReadResultsAsync().WithCancellation(cancellationToken))
        {
            using (element)
            {
                yield return await element.ToJsonAsync();
            }
        }
    }
}