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
using Microsoft.Extensions.Options;

namespace Locker.Testing.Services;

public class TokenServiceTest
{
    private const string SECRET = "ea34d28cf4c89b42e86defd2ae79209d3a91c1d30db332332b5597f9cbf9092c1b5838b1a816cde9ac4ba47c381b1dd60c93db0ee1100b032195efc6e4b43701";
    private const string ISSUER = "issuer";

    private static readonly DbContextFactoryMock _mock = new();
    private readonly ITokenService _token;

    public static readonly IEnumerable<object[]> Data =
        _mock.DataContextMock.Tenants.SelectMany(tenant =>
            _mock.DataContextMock.Users.Select(user =>
                new object[] { tenant, user }
            )
        );

    public TokenServiceTest()
    {
        var options = Options.Create(new LockerOptions()
        {
            SecretKey = SECRET,
            Issuer = ISSUER,
        });

        _token = new TokenService(_mock.IDbContextFactory, options);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public async Task CreateAccessTokenTest(Tenant tenant, User user)
    {
        var accessToken = await _token.BuildAccessTokenAsync(user, tenant.ID);

        var jwt = _token.Encode(accessToken);
        var (principal, token) = _token.Decode(jwt);

        Assert.Equal(ISSUER, token.Issuer);
        Assert.Equal(user.ID, principal.GetID());
        Assert.Equal(user.Email, principal.GetEmail());
        Assert.Equal(user.Phone, principal.GetPhone());
        Assert.Equal($"{user.FirstName} {user.LastName}", principal.GetName());
        Assert.Equal(tenant.ID, principal.GetTenantID());
        Assert.Equal(WellKnownRoles.User, principal.GetAccessLevel());
        Assert.Contains(WellKnownRoles.User, principal.GetRoles());
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void CreateRefreshTokenTest(Tenant tenant, User user)
    {
        var refreshToken = _token.BuildRefreshToken(user, tenant.ID);

        var jwt = _token.Encode(refreshToken);
        var (principal, token) = _token.Decode(jwt);

        Assert.Equal(ISSUER, token.Issuer);
        Assert.Equal(user.ID, principal.GetID());
        Assert.Equal(user.Email, principal.GetEmail());
        Assert.Equal(user.Phone, principal.GetPhone());
        Assert.Equal($"{user.FirstName} {user.LastName}", principal.GetName());
        Assert.Equal(tenant.ID, principal.GetTenantID());
    }
}