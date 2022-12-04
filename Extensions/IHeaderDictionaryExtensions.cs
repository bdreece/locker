namespace Locker;

public static class IHeaderDictionaryExtensions
{
    public static void AddRefreshTokenCookie(
        this IHeaderDictionary dictionary,
        string refreshToken,
        string context
    ) => dictionary.Add(
        "Set-Cookie",
        $"locker_{context}_refresh={refreshToken}; Path=/; HttpOnly; SameSite=None; Secure"
    );
}