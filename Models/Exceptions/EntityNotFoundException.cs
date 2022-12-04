namespace Locker;

public class EntityNotFoundException : NullReferenceException
{
    public EntityNotFoundException(Type type) : base($"{type.Name} not found!") { }
}