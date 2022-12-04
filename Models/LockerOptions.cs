namespace Locker;

public class LockerOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string SendGridApiKey { get; set; } = string.Empty;
}