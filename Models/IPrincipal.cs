namespace Locker.Models;

public interface IPrincipal
{
    string ID { get; }
    string Name { get; }
    string SecurityStamp { get; }
}