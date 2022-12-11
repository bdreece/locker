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