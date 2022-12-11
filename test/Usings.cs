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
global using System.Runtime.CompilerServices;
global using System.Text.Json;
global using Xunit;
global using Microsoft.Extensions.DependencyInjection;
global using Moq;
global using Moq.EntityFrameworkCore;
global using HotChocolate;
global using HotChocolate.Execution;
global using Locker.Models;
global using Locker.Models.Entities;
global using Locker.Resolvers;
global using Locker.Services;
global using Locker.Testing.Mocks;