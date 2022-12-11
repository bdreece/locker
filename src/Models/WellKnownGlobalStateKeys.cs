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
using static Locker.Models.WellKnownRoles;

namespace Locker.Models;

public static class WellKnownGlobalStateKeys
{
    public const string TenantID = "TENANT_ID";
    public const string TenantKey = "TENANT_KEY";
    public const string TenantAuthState = "TENANT_AUTH_STATE";
    public const string IsUser = User;
    public const string IsAdmin = Admin;
    public const string IsRoot = Root;
}