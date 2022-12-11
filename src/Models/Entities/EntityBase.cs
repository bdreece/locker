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

namespace Locker.Models.Entities;

[Node]
public abstract class EntityBase
{
    [ID]
    [GraphQLName("id")]
    [Key]
    public string ID { get; init; } = Guid.NewGuid().ToString();

    [Required]
    public DateTime DateCreated { get; init; } = DateTime.UtcNow;

    [Required]
    public DateTime DateLastUpdated { get; set; } = DateTime.UtcNow;

    public void Update()
    {
        DateLastUpdated = DateTime.UtcNow;
    }
}