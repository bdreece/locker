namespace Locker.Tests;

public static class IEnumerableExtensions
{
    public static Stack<T> ToStack<T>(this IEnumerable<T> source) =>
        new(source.Reverse());
}