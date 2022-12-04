namespace Locker;

public class MissingOneOfException : Exception
{
    public MissingOneOfException(string param1, string param2)
        : base($"Must have one of {param1} or {param2}") { }
}