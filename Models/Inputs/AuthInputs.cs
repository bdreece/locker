namespace Locker.Models.Inputs;

public record class LoginInput(string? Email, string? Phone, string Password);

public record class RegisterInput(
    string FirstName,
    string LastName,
    string? Phone,
    string? Email,
    string Password
);