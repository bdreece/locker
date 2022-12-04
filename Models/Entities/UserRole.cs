using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Locker.Services;

namespace Locker.Models.Entities;

[Index(nameof(Context))]
public sealed class UserRole : EntityBase
{
    [Required]
    [ID(nameof(User))]
    [ForeignKey(nameof(User))]
    public string UserID { get; init; } = string.Empty;
    public User? User { get; init; }

    [Required]
    [ID(nameof(Role))]
    [ForeignKey(nameof(Role))]
    public string RoleID { get; init; } = string.Empty;
    public Role? Role { get; init; }

    [Required]
    public string Context { get; init; } = string.Empty;

    public static Task<UserRole> Get([ID] string id, DataContext db) =>
        db.UserRoles.SingleAsync(userRole => userRole.ID == id);
}