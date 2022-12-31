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

public sealed class TokenServiceTests : DataFixture
{
    private static readonly IEnumerable<int> Cases = Enumerable.Range(1, 3);

    private const string ISSUER = "test_issuer";

    private ITokenService? tokenService { get; set; }

    [OneTimeSetUp]
    public void Setup()
    {
        var users = Fixture.Build<User>()
            .Without(u => u.Accounts)
            .CreateMany()
            .ToArray();

        var roles = Fixture.Build<Role>()
            .Without(r => r.Accounts)
            .Without(r => r.Tenant)
            .CreateMany()
            .ToArray();

        var roleStack = roles.ToStack();

        var tenants = Fixture.Build<Tenant>()
            .With(t => t.Roles, () => new[] { roleStack.Pop() })
            .Without(t => t.Accounts)
            .CreateMany()
            .ToArray();

        roleStack = roles.ToStack();

        var accounts = Cases.Select(i => new Account
        {
            ID = i.ToString(),
            RoleID = roles[i - 1].ID,
            Role = roles[i - 1],
            UserID = users[i - 1].ID,
            User = users[i - 1],
            TenantID = tenants[i - 1].ID,
            Tenant = tenants[i - 1]
        });

        DataContextMock.Setup(db => db.Accounts).ReturnsDbSet(accounts);
        InitServices();

        var tokenOptions = Fixture.Build<LockerOptions>()
            .With(o => o.Issuer, ISSUER)
            .Create();

        var optionsMock = new Mock<IOptions<LockerOptions>>();
        optionsMock.Setup(o => o.Value).Returns(tokenOptions);

        tokenService = new TokenService(DbContextFactory, optionsMock.Object);
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    [TestOf(typeof(TokenService))]
    public async Task TestBuildAccessTokenAsync(int id)
    {
        var account = await DataContext.Accounts
            .Include(a => a.User)
            .Include(a => a.Role)
            .Include(a => a.Tenant)
            .FirstOrDefaultAsync(a => a.ID == id.ToString());

        Assume.That(account, Is.Not.Null);
        Assume.That(account!.User, Is.Not.Null);
        Assume.That(account.Role, Is.Not.Null);
        Assume.That(account.Tenant, Is.Not.Null);

        var token = await tokenService!.BuildAccessTokenAsync(account.User!, account.Tenant!.ID);

        Assert.That(token.Issuer, Is.EqualTo(ISSUER));

        var jwt = tokenService.Encode(token);
        var (principal, decoded) = tokenService.Decode(jwt);

        Assert.Multiple(() =>
        {
            Assert.That(decoded.Id, Is.EqualTo(token.Id));

            Assert.That(principal.GetID(), Is.EqualTo(account.User!.ID));
            Assert.That(principal.GetName(),
                Is.EqualTo($"{account.User!.FirstName} {account.User.LastName}"));
            Assert.That(principal.GetEmail(), Is.EqualTo(account.User.Email));
            Assert.That(principal.GetPhone(), Is.EqualTo(account.User.Phone));
            Assert.That(principal.GetRoles(), Is.EquivalentTo(new[] { account.Role!.Name }));
        });
    }

    [Test]
    [TestCaseSource(nameof(Cases))]
    [TestOf(nameof(TokenService))]
    public async Task TestBuildRefreshToken(int id)
    {
        var account = await DataContext.Accounts
            .Include(a => a.User)
            .Include(a => a.Role)
            .Include(a => a.Tenant)
            .FirstOrDefaultAsync(a => a.ID == id.ToString());

        Assume.That(account, Is.Not.Null);
        Assume.That(account!.User, Is.Not.Null);
        Assume.That(account.Role, Is.Not.Null);
        Assume.That(account.Tenant, Is.Not.Null);

        var token = tokenService!.BuildRefreshToken(account.User!, account.Tenant!.ID);

        Assert.That(token.Issuer, Is.EqualTo(ISSUER));

        var jwt = tokenService.Encode(token);
        var (principal, decoded) = tokenService.Decode(jwt);

        Assert.Multiple(() =>
        {
            Assert.That(decoded.Id, Is.EqualTo(token.Id));

            Assert.That(principal.GetID(), Is.EqualTo(account.User!.ID));
            Assert.That(principal.GetName(),
                Is.EqualTo($"{account.User!.FirstName} {account.User.LastName}"));
            Assert.That(principal.GetEmail(), Is.EqualTo(account.User.Email));
            Assert.That(principal.GetPhone(), Is.EqualTo(account.User.Phone));
        });
    }
}