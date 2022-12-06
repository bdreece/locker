using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Locker.Services;

namespace Locker.Models.Entities;

public sealed class Role : EntityBase
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public IList<Account> Accounts { get; set; } = new List<Account>();

    public static Task<Role> Get([ID] string id, DataContext db) =>
        db.Roles.SingleAsync(role => role.ID == id);
}