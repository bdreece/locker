using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace Locker.Models.Entities;

public abstract class AuthEntityBase : EntityBase, IPrincipal
{
    [Required]
    public string SecurityStamp { get; set; } = GenerateSecurityStamp();

    public abstract string Name { get; set; }

    protected void UpdateSecurityStamp()
    {
        SecurityStamp = GenerateSecurityStamp();
    }

    private static string GenerateSecurityStamp()
    {
        using var rng = RandomNumberGenerator.Create();
        var stamp = new byte[128];
        rng.GetBytes(stamp);
        return Convert.ToBase64String(stamp);
    }
}