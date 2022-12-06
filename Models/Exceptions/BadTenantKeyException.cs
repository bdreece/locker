namespace Locker;

public class BadTenantKeyException : ArgumentException
{
    public BadTenantKeyException() : base("Bad tenant key!") { }
}