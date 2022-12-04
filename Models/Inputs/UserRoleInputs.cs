namespace Locker.Models.Inputs;

public record class CreateUserRoleInput(string RoleID, string UserID, string Context);