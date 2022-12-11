namespace Locker;

public enum TokenType
{
    AccessToken,
    RefreshToken,
}

public class MissingTokenException : ArgumentNullException
{
    public MissingTokenException(TokenType type) : base(type.ToString()) { }
}