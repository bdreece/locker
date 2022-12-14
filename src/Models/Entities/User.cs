/**
 * locker - A multi-tenant GraphQL authentication & authorization server
 * Copyright (C) 2022 Brian Reece

 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Locker.Models.Inputs;
using Locker.Services;

namespace Locker.Models.Entities;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Phone), IsUnique = true)]
public sealed class User : EntityBase
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    public string? Email { get; set; }
    public string? Phone { get; set; }

    [Required]
    [GraphQLIgnore]
    public string Hash { get; set; } = string.Empty;

    [Required]
    public string SecurityStamp { get; set; } = GenerateSecurityStamp();

    public IList<Account> Accounts { get; set; } = new List<Account>();

    public void UpdateHash(string hash)
    {
        Hash = hash;
        UpdateSecurityStamp();
    }

    public void Update(UpdateUserInput input)
    {
        base.Update();

        FirstName = input.FirstName ?? FirstName;
        LastName = input.LastName ?? LastName;
        Email = input.Email ?? Email;
        Phone = input.Phone ?? Phone;

        if (input.Email is not null || input.Phone is not null)
            UpdateSecurityStamp();
    }

    private void UpdateSecurityStamp()
    {
        SecurityStamp = GenerateSecurityStamp();
    }

    public static Task<User> Get([ID] string id, DataContext db) =>
        db.Users.SingleAsync(u => u.ID == id);

    private static string GenerateSecurityStamp()
    {
        using var rng = RandomNumberGenerator.Create();
        var stamp = new byte[128];
        rng.GetBytes(stamp);
        return Convert.ToBase64String(stamp);
    }
}