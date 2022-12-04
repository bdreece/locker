namespace Locker;

public class UnauthenticatedException : Exception
{
    public UnauthenticatedException()
        : base("Could not authenticate request!") { }
}