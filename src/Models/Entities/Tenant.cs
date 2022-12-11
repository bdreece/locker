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

    public IList<Account> Accounts { get; init; } = new List<Account>();
    public IList<Role> Roles { get; init; } = new List<Role>();

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