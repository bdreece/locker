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
global using NUnit.Framework;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using AutoFixture;
global using HotChocolate;
global using HotChocolate.Execution;
global using HotChocolate.Types.Relay;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Options;
global using Microsoft.EntityFrameworkCore;
global using Moq;
global using Moq.EntityFrameworkCore;
global using Snapshooter.NUnit;

global using Locker.Models.Entities;
global using Locker.Models.Inputs;
global using Locker.Services;
global using Locker.Resolvers;