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
using Microsoft.EntityFrameworkCore;

namespace Locker.Testing.Mocks;

public class DbContextFactoryMock
{
    private readonly Mock<IDbContextFactory<DataContext>> _mock = new();

    public IDbContextFactory<DataContext> IDbContextFactory { get => _mock.Object; }
    public DataContextMock DataContextMock { get; } = new();

    public DbContextFactoryMock()
    {
        _mock.Setup(factory => factory.CreateDbContext()).Returns(DataContextMock.DataContext);
    }
}