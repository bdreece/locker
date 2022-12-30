namespace Locker.Tests;

[TestFixture]
public abstract class FixtureBase
{
    protected Fixture Fixture { get; } = new();
}