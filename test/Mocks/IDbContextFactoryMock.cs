using Microsoft.EntityFrameworkCore;

namespace Locker.Testing.Mocks;

public class DbContextFactoryMock
{
    private readonly Mock<IDbContextFactory<DataContext>> _mock = new();

    public IDbContextFactory<DataContext> IDbContextFactory { get => _mock.Object; }
    public DataContextMock DataContextMock { get; } = new();

    public DbContextFactoryMock()
    {
        _mock.Setup(factory => factory.CreateDbContext()).Returns(DataContextMock.DataContext);
    }
}