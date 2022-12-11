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
namespace Locker.Testing.Resolvers;

public class AccountResolverTest
{
    private readonly DataContextMock _mock = new();
    private readonly Query query = new();

    private const string ACCOUNT = @"
    fragment account on Account {
        id
        dateCreated
        dateLastUpdated
        roleID
        userID
        tenantID
    }
    ";

    private record AccountsResult(AccountConnection? Data, dynamic Errors);
    private record AccountConnection(IEnumerable<Account> Nodes);
    private record AccountResult(Account? Data);

    [Fact]
    public async Task GetAccountsTest()
    {
        var query = @$"
        {ACCOUNT}
        {{
            accounts {{
                nodes {{
                    ...account
                }}
            }}
        }}
        ";

        // Console.WriteLine(query);

        var expected = _mock.Accounts;
        var gotJson = await ServiceMock.ExecuteRequestAsync(b => b.SetQuery(query));
        var got = JsonSerializer.Deserialize<AccountsResult>(gotJson);

        Assert.Equal(expected, got?.Data?.Nodes ?? Enumerable.Empty<Account>());
    }

    [Fact]
    public async void GetFirstAccountTest()
    {
        var query = @$"
        {ACCOUNT}
        {{
            firstAccount {{
                ...account
            }}
        }}
        ";

        // Console.WriteLine(query);

        var expected = _mock.Accounts.First();
        var gotJson = await ServiceMock.ExecuteRequestAsync(b => b.SetQuery(query));
        var got = JsonSerializer.Deserialize<AccountResult>(gotJson);

        Assert.Equal(expected, got?.Data);
    }

    [Fact]
    public async void GetUniqueAccountTest()
    {
        var expected = _mock.Accounts.First();
        var query = @$"
            {ACCOUNT}
            {{
                uniqueAccount(where: {{ id: {{ eq: ""{expected.ID}"" }} }}) {{
                    ...account
                }}
            }}
            ";

        // Console.WriteLine(query);

        var gotJson = await ServiceMock.ExecuteRequestAsync(b => b.SetQuery(query));
        var got = JsonSerializer.Deserialize<AccountResult>(gotJson);

        Assert.Equal(expected, got?.Data);
    }
}