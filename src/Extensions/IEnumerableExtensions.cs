namespace Locker;

public static class IEnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            action(item);
        }
    }

    public static async Task<IEnumerable<T>> WhenAll<T>(this IEnumerable<Task<T>> enumerable) =>
        await Task.WhenAll(enumerable);
}