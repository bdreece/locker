namespace Locker.Models.Inputs;

public record UpdateUserInput(
    string? FirstName,
    string? LastName,
    string? Phone,
    string? Email
);

public record CreateServiceInput(string Name);

public record CreateRoleInput(string Name);

public record CreateAccountInput(string RoleID, string UserID, string? TenantID);
