namespace Locker;

public class MissingOneOfException : Exception
{
    public MissingOneOfException(params string[] param)
        : base($"Must have one of {param.Select(p => $"{p}, ")}") { }
}