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
using static Locker.Models.WellKnownGlobalStateKeys;

namespace Locker.Tests;

public sealed class AccountResolverTests : ResolverFixture
{
    private static readonly IEnumerable<int> Cases = Enumerable.Range(1, 3);

    private const bool DEBUG = true;
    private const string ACCOUNT_FRAGMENT = @"
        fragment account on Account {
            id
            userID
            roleID
            tenantID
        }
    ";

    private record AccountData(
        IEnumerable<string> Ids,
        IEnumerable<string> UserIds,
        IEnumerable<string> RoleIds,
        IEnumerable<string> TenantIds
    )
    {
        public AccountData()
            : this(
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>(),
                Enumerable.Empty<string>()
            )
        { }
    };

    [OneTimeSetUp]
    public void Setup()
    {
        var (ids, userIds, roleIds, tenantIds) = Cases.Aggregate(
            new AccountData(),
            (ids, i) =>
                new(
                    ids.Ids.Append(i.ToString()),
                    ids.UserIds.Append((i + 1).ToString()),
                    ids.RoleIds.Append((i + 2).ToString()),
                    ids.TenantIds.Append((i + 3).ToString())
                ),
            (ids) =>
                (
                    ids.Ids.ToStack(),
                    ids.UserIds.ToStack(),
                    ids.RoleIds.ToStack(),
                    ids.TenantIds.ToStack()
                )
            );

        var tenants = Fixture.Build<Tenant>()
            .Without(t => t.Accounts)
            .Without(t => t.Roles)
            .CreateMany();

        var accounts = Fixture.Build<Account>()
            .With(a => a.ID, () => ids.Pop())
            .With(a => a.UserID, () => userIds.Pop())
            .With(a => a.RoleID, () => roleIds.Pop())
            .With(a => a.TenantID, () => tenantIds.Pop())
            .Without(a => a.User)
            .Without(a => a.Role)
            .Without(a => a.Tenant)
            .CreateMany();

        DataContextMock.Setup(db => db.Accounts).ReturnsDbSet(accounts);
        DataContextMock.Setup(db => db.Tenants).ReturnsDbSet(tenants);
        InitServices();
    }

    [Test]
    [TestOf(nameof(Query))]
    public async Task TestGetAccounts()
    {
        await using var scope = ServiceProvider!.CreateAsyncScope();

        var query = @$"
            {ACCOUNT_FRAGMENT} 
            query GetAccounts {{
                accounts {{
                    nodes {{
                        ...account
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
    public async Task TestGetFirstAccount(int id)
    {
        await using var scope = ServiceProvider!.CreateAsyncScope();

        var query = @$"
            {ACCOUNT_FRAGMENT} 
            query GetFirstAccount($id: ID!) {{
                firstAccount(where: {{
                    id: {{
                        eq: $id
                    }}
                }}) {{
                    ...account
                }}
            }}
        ";

        var tenant = await DataContext.Tenants.FirstOrDefaultAsync();
        Assume.That(tenant, Is.Not.Null);

        var idString = IdSerializer!.Serialize(Schema.DefaultName, nameof(Account), id);
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
    public async Task TestGetUniqueAccount(int id)
    {
        await using var scope = ServiceProvider!.CreateAsyncScope();

        var query = @$"
            {ACCOUNT_FRAGMENT} 
            query GetUniqueAccount($id: ID!) {{
                uniqueAccount(where: {{
                    id: {{
                        eq: $id
                    }}
                }}) {{
                    ...account
                }}
            }}
        ";

        var tenant = await DataContext.Tenants.FirstOrDefaultAsync();
        Assume.That(tenant, Is.Not.Null);

        var idString = IdSerializer!.Serialize(Schema.DefaultName, nameof(Account), id);
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