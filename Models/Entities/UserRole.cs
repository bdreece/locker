using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Locker.Services;

namespace Locker.Models.Entities;

public sealed class UserRole : EntityBase
{
    [Required]
    [ForeignKey(nameof(User))]
    public string UserID { get; init; } = string.Empty;
    public User? User { get; init; }

    [Required]
    [ForeignKey(nameof(Role))]
    public string RoleID { get; init; } = string.Empty;
    public Role? Role { get; init; }

    [Required]
    public string Context { get; init; } = string.Empty;

    public static Task<UserRole> Get(string id, DataContext db) =>
        db.UserRoles.SingleAsync(userRole => userRole.ID == id);
}