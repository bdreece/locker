using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Locker.Services;

namespace Locker.Models.Entities;

public sealed class Account : EntityBase
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
    [ID(nameof(Tenant))]
    [ForeignKey(nameof(Tenant))]
    public string TenantID { get; init; } = string.Empty;
    public Tenant? Tenant { get; init; }

    public static Task<Account> Get([ID] string id, DataContext db) =>
        db.Accounts.SingleAsync(account => account.ID == id);
}