using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using Locker.Services;

namespace Locker.Models.Entities;

public sealed class Service : AuthEntityBase
{
    [Required]
    public override string Name { get; set; } = string.Empty;

    [Required]
    public string Context { get; set; } = string.Empty;

    [Required]
    [Authorize(Roles = new[] { WellKnownRoles.Root })]
    public string ApiKey { get; private set; } = GenerateApiKey();

    public void UpdateApiKey()
    {
        ApiKey = GenerateApiKey();
        UpdateSecurityStamp();
        base.Update();
    }

    public static Task<Service> GetAsync([ID] string id, DataContext db) =>
        db.Services.SingleAsync(s => s.ID == id);

    private static string GenerateApiKey()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[128];
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}