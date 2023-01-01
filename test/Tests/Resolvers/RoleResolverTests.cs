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

public sealed class RoleResolverTests : ResolverFixture
{
    private static readonly IEnumerable<int> Cases = Enumerable.Range(1, 3);

    private const bool DEBUG = true;
    private const string ROLE_FRAGMENT = @"
        fragment role on Role {
            id
            name
            tenantID
        }
    ";

    private record RoleData(
        IEnumerable<string> Ids,
        IEnumerable<string> Names,
        IEnumerable<string> TenantIds
    )
    {
        public RoleData()
            : this(
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>()
            )
        { }
    };

    [OneTimeSetUp]
    public void Setup()
    {
        var (ids, names, tenantIds) = Cases.Aggregate(
            new RoleData(),
            (data, i) =>
                new(
                    data.Ids.Append(i.ToString()),
                    data.Names.Append($"Name{i}"),
                    data.TenantIds.Append((i + 2).ToString())
                ),
            data =>
                (
                    data.Ids.ToStack(),
                    data.Names.ToStack(),
                    data.TenantIds.ToStack()
                )
            );

        var tenants = Fixture.Build<Tenant>()
            .Without(t => t.Accounts)
            .Without(t => t.Roles)
            .CreateMany();

        var roles = Fixture.Build<Role>()
            .With(r => r.ID, () => ids.Pop())
            .With(r => r.Name, () => names.Pop())
            .With(r => r.TenantID, () => tenantIds.Pop())
            .Without(r => r.Tenant)
            .Without(r => r.Accounts)
            .CreateMany();

        DataContextMock.Setup(db => db.Roles).ReturnsDbSet(roles);
        DataContextMock.Setup(db => db.Tenants).ReturnsDbSet(tenants);
        InitServices();
    }

    [Test]
    [TestOf(nameof(Query))]
    public async Task TestGetRoles()
    {
        await using var scope = ServiceProvider!.CreateAsyncScope();

        var query = @$"
            {ROLE_FRAGMENT} 
            query GetRoles {{
                roles {{
                    nodes {{
                        ...role
                    }}
                }}
            }}
        ";

        var tenant = await DataContext.Tenants.FirstOrDefaultAsync();
        Assume.That(tenant, Is.Not.Null);

        var request = new QueryRequestBuilder()
            .SetQuery(query)
            .SetServices(scope.ServiceProvider)
            .SetProperties(new()
            {
                { TenantID, tenant!.ID },
                { TenantKey, tenant.ApiKey },
            })
            .Create();

        var result = await Executor!.ExecuteAsync(request);
        var errors = result.Errors?.AsEnumerable() ?? Enumerable.Empty<IError>();

        Assert.That(errors, Is.Empty,
            JsonSerializer.Serialize(errors, JsonOptions));

        result.MatchSnapshot();
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    [TestOf(nameof(Query))]
    public async Task TestGetFirstRole(int id)
    {
        await using var scope = ServiceProvider!.CreateAsyncScope();

        var query = @$"
            {ROLE_FRAGMENT} 
            query GetFirstRole($id: ID!) {{
                firstRole(where: {{
                    id: {{
                        eq: $id
                    }}
                }}) {{
                    ...role
                }}
            }}
        ";

        var tenant = await DataContext.Tenants.FirstOrDefaultAsync();
        Assume.That(tenant, Is.Not.Null);

        var idString = IdSerializer!.Serialize(Schema.DefaultName, nameof(Role), id);
        var request = new QueryRequestBuilder()
            .SetQuery(query)
            .SetServices(scope.ServiceProvider)
            .SetVariableValue(nameof(id), idString)
            .SetProperties(new()
            {
                { TenantID, tenant!.ID },
                { TenantKey, tenant.ApiKey },
            })
            .Create();

        var result = await Executor!.ExecuteAsync(request);
        var errors = result.Errors?.AsEnumerable() ?? Enumerable.Empty<IError>();

        Assert.That(errors, Is.Empty,
            JsonSerializer.Serialize(errors, JsonOptions));

        result.MatchSnapshot();
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    [TestOf(nameof(Query))]
    public async Task TestGetUniqueRole(int id)
    {
        await using var scope = ServiceProvider!.CreateAsyncScope();

        var query = @$"
            {ROLE_FRAGMENT} 
            query GetUniqueRole($id: ID!) {{
                uniqueRole(where: {{
                    id: {{
                        eq: $id
                    }}
                }}) {{
                    ...role
                }}
            }}
        ";

        var tenant = await DataContext.Tenants.FirstOrDefaultAsync();
        Assume.That(tenant, Is.Not.Null);

        var idString = IdSerializer!.Serialize(Schema.DefaultName, nameof(Role), id);
        var request = new QueryRequestBuilder()
            .SetQuery(query)
            .SetServices(scope.ServiceProvider)
            .SetVariableValue(nameof(id), idString)
            .SetProperties(new()
            {
                { TenantID, tenant!.ID },
                { TenantKey, tenant.ApiKey },
            })
            .Create();

        var result = await Executor!.ExecuteAsync(request);
        var errors = result.Errors?.AsEnumerable() ?? Enumerable.Empty<IError>();

        Assert.That(errors, Is.Empty,
            JsonSerializer.Serialize(errors, JsonOptions));

        result.MatchSnapshot();
    }
}