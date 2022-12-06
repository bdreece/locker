using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

using Locker.Services;

namespace Locker.Models.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Tenant : EntityBase
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [GraphQLIgnore]
    public string ApiKey { get; set; } = GenerateApiKey();

    [Required]
    [ID(nameof(Role))]
    [ForeignKey(nameof(UserRole))]
    public string UserRoleID { get; set; } = string.Empty;
    public Role? UserRole { get; set; }

    [Required]
    [ID(nameof(Role))]
    [ForeignKey(nameof(AdminRole))]
    public string AdminRoleID { get; set; } = string.Empty;
    public Role? AdminRole { get; set; }

    public IList<Account> Accounts { get; init; } = new List<Account>();

    public static Task<Tenant> Get([ID] string id, DataContext db) =>
        db.Tenants.SingleAsync(t => t.ID == id);

    private static string GenerateApiKey()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[128];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}