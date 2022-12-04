namespace Locker.Models.Inputs;

public record class UpdateUserInput(
    string? FirstName,
    string? LastName,
    string? Phone,
    string? Email
);