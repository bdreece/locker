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

public sealed class UserResolverTests : ResolverFixture
{
    private static readonly IEnumerable<int> Cases = Enumerable.Range(1, 3);

    private const bool DEBUG = true;
    private const string USER_FRAGMENT = @"
        fragment user on User {
            id
            firstName
            lastName
            email
            phone
        }
    ";

    private record UserData(
        IEnumerable<string> Ids,
        IEnumerable<string> FirstNames,
        IEnumerable<string> LastNames,
        IEnumerable<string> Emails,
        IEnumerable<string> Phones
    )
    {
        public UserData()
            : this(
                Enumerable.Empty<string>(),
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
        var (ids, firstNames, lastNames, emails, phones) = Cases.Aggregate(
            new UserData(),
            (data, i) =>
                new(
                    data.Ids.Append(i.ToString()),
                    data.FirstNames.Append($"FirstName{i}"),
                    data.LastNames.Append($"FirstName{i}"),
                    data.Emails.Append($"Email{i}"),
                    data.Phones.Append($"Phone{i}")
                ),
            (data) =>
                (
                    data.Ids.ToStack(),
                    data.FirstNames.ToStack(),
                    data.LastNames.ToStack(),
                    data.Emails.ToStack(),
                    data.Phones.ToStack()
                )
            );

        var tenants = Fixture.Build<Tenant>()
            .Without(t => t.Accounts)
            .Without(t => t.Roles)
            .CreateMany();

        var users = Fixture.Build<User>()
            .With(u => u.ID, () => ids.Pop())
            .With(u => u.FirstName, () => firstNames.Pop())
            .With(u => u.LastName, () => lastNames.Pop())
            .With(u => u.Email, () => emails.Pop())
            .With(u => u.Phone, () => phones.Pop())
            .Without(u => u.Accounts)
            .CreateMany();

        DataContextMock.Setup(db => db.Users).ReturnsDbSet(users);
        DataContextMock.Setup(db => db.Tenants).ReturnsDbSet(tenants);
        InitServices();
    }

    [Test]
    [TestOf(nameof(Query))]
    public async Task TestGetUsers()
    {
        await using var scope = ServiceProvider!.CreateAsyncScope();

        var query = @$"
            {USER_FRAGMENT} 
            query GetUsers {{
                users {{
                    nodes {{
                        ...user
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
    public async Task TestGetFirstUser(int id)
    {
        await using var scope = ServiceProvider!.CreateAsyncScope();

        var query = @$"
            {USER_FRAGMENT} 
            query GetFirstUser($id: ID!) {{
                firstUser(where: {{
                    id: {{
                        eq: $id
                    }}
                }}) {{
                    ...user
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
    public async Task TestGetUniqueUser(int id)
    {
        await using var scope = ServiceProvider!.CreateAsyncScope();

        var query = @$"
            {USER_FRAGMENT} 
            query GetUniqueUser($id: ID!) {{
                uniqueUser(where: {{
                    id: {{
                        eq: $id
                    }}
                }}) {{
                    ...user
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