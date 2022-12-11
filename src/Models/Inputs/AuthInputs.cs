namespace Locker.Models.Inputs;

public record LoginInput(string? Email, string? Phone, string Password);

public record RegisterInput(
    string FirstName,
    string LastName,
    string? Phone,
    string? Email,
    string Password
);