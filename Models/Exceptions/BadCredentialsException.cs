namespace Locker;

public class BadCredentialsException : ArgumentException
{
    public BadCredentialsException() : base("Bad credentials!") { }
}