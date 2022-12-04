namespace Locker;

public class EntityConflictException : Exception
{
    public EntityConflictException(Type type) : base($"{type.Name} already exists!") { }
}