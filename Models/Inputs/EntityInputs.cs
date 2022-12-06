namespace Locker.Models.Inputs;

public record class UpdateUserInput(
    string? FirstName,
    string? LastName,
    string? Phone,
    string? Email
);

public record class CreateServiceInput(string Name);

public record class CreateRoleInput(string Name);

public record struct CreateAccountInput(string RoleID, string UserID, string? TenantID);
