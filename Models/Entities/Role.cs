using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Locker.Services;

namespace Locker.Models.Entities;

public sealed class Role : EntityBase
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public IList<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public static Task<Role> Get(string id, DataContext db) =>
        db.Roles.SingleAsync(role => role.ID == id);
}