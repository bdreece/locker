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
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

using Locker.Services;

namespace Locker.Models.Entities;

[Index(nameof(Name), IsUnique = true)]
public sealed class Tenant : EntityBase
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