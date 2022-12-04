namespace Locker.Models.Inputs;

public record class LoginInput(string? Email, string? Phone, string Password, string Context);

public record class RefreshInput(string Context);

public record class RegisterInput(
    string FirstName,
    string LastName,
    string? Phone,
    string? Email,
    string Password,
    string Context
);