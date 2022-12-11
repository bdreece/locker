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