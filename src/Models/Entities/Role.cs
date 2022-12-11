using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Locker.Services;

namespace Locker.Models.Entities;

public sealed class Role : EntityBase
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [ID(nameof(Tenant))]
    [ForeignKey(nameof(Tenant))]
    public string? TenantID { get; set; }
    public Tenant? Tenant { get; set; }

    public IList<Account> Accounts { get; set; } = new List<Account>();

    public static Task<Role> Get([ID] string id, DataContext db) =>
        db.Roles.SingleAsync(role => role.ID == id);
}