namespace Locker.Testing.Mocks;

public static class ServiceMock
{
    public static IServiceProvider Services { get; }
    public static RequestExecutorProxy Executor { get; }
    public static DataContextMock DataContextMock { get; } = new();

    static ServiceMock()
    {
        Services = new ServiceCollection()
            .AddGraphQLServer()
            .AddQueryType<Query>()
            .Services
            .AddSingleton<DataContext>(DataContextMock.DataContext)
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