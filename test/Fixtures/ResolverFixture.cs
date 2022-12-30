namespace Locker.Tests;

public abstract class ResolverFixture : DataFixture
{
    protected IServiceProvider? ServiceProvider { get; private set; }
    protected RequestExecutorProxy? Executor { get; private set; }

    protected override void InitServices()
    {
        base.InitServices();
        ServiceProvider = new ServiceCollection()
            .AddScoped(_ => DataContext)
            .AddGraphQL()
            .AddMutationConventions(applyToAllMutations: true)
            .AddQueryFieldToMutationPayloads()
            .RegisterDbContext<DataContext>()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .Services
            .AddSingleton(sp => new RequestExecutorProxy(
                sp.GetRequiredService<IRequestExecutorResolver>(),
                Schema.DefaultName
            ))
            .BuildServiceProvider();

        Executor = ServiceProvider.GetRequiredService<RequestExecutorProxy>();
    }
}