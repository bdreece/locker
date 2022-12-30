namespace Locker.Tests;

public abstract class DataFixture : FixtureBase
{
    protected Mock<IDbContextFactory<DataContext>> DbContextFactoryMock { get; } = new();
    protected Mock<DataContext> DataContextMock { get; } = new();
    protected DataContext DataContext => DataContextMock.Object;
    protected IDbContextFactory<DataContext> DbContextFactory => DbContextFactoryMock.Object;

    protected virtual void InitServices()
    {
        DbContextFactoryMock.Setup(f => f.CreateDbContext()).Returns(DataContext);
    }
}