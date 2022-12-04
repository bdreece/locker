namespace Locker;

public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base("Request unauthorized!") { }
}