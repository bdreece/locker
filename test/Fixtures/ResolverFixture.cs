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
namespace Locker.Tests;

public abstract class ResolverFixture : DataFixture
{
    protected IServiceProvider? ServiceProvider { get; private set; }
    protected IIdSerializer? IdSerializer { get; private set; }
    protected RequestExecutorProxy? Executor { get; private set; }

    protected ResolverFixture() : base() { }

    protected override void InitServices()
    {
        base.InitServices();
        ServiceProvider = new ServiceCollection()
            .AddScoped(_ => DbContextFactory)
            .AddGraphQL()
            .AddGlobalObjectIdentification()
            .AddMutationConventions(applyToAllMutations: true)
            .AddQueryFieldToMutationPayloads()
            .RegisterDbContext<DataContext>(DbContextKind.Pooled)
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
        IdSerializer = ServiceProvider.GetRequiredService<IIdSerializer>();
    }
}