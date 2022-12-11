using static Locker.Models.WellKnownRoles;

namespace Locker.Testing.Mocks;

public class DataContextMock
{
    private readonly Mock<DataContext> _mock = new();

    public DataContext DataContext { get => _mock.Object; }

    public IEnumerable<Tenant> Tenants { get; set; }
    public IEnumerable<User> Users { get; set; }
    public IEnumerable<Account> Accounts { get; set; }
    public IEnumerable<Role> Roles { get; set; }

    public DataContextMock()
    {
        Tenants = new Tenant[]
        {
            new() { Name = "Tenant1" },
            new() { Name = "Tenant2" },
            new() { Name = "Tenant3" }
        };

        Users = new User[]
        {
            new()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "johndoe@email.com",
                Phone = "5555555555",
                Hash = "johnhash"
            },
            new()
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "janedoe@email.com",
                Phone = "6666666666",
                Hash = "janehash"
            },
            new()
            {
                FirstName = "Jack",
                LastName = "Doe",
                Email = "janedoe@email.com",
                Phone = "7777777777",
                Hash = "janehash"
            }
        };

        var role = new Role() { Name = WellKnownRoles.User };
        Roles = new Role[]
        {
            role,
            new() { Name = Admin },
            new() { Name = Root }
        };

        Accounts = Users.SelectMany(user =>
            Tenants.Select(tenant => new Account()
            {
                Role = role,
                RoleID = role.ID,
                Tenant = tenant,
                TenantID = tenant.ID,
                User = user,
                UserID = user.ID
            })
        );

        _mock.Setup(db => db.Tenants).ReturnsDbSet(Tenants);
        _mock.Setup(db => db.Users).ReturnsDbSet(Users);
        _mock.Setup(db => db.Roles).ReturnsDbSet(Roles);
        _mock.Setup(db => db.Accounts).ReturnsDbSet(Accounts);
    }
}