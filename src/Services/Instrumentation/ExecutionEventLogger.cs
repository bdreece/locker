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
using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using HotChocolate.Resolvers;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Locker.Services;

public class ExecutionEventLogger : ExecutionDiagnosticEventListener
{
    private readonly ILogger _logger = Log.Logger.ForContext<ExecutionEventLogger>();

    public override void SyntaxError(IRequestContext context, IError error)
    {
        if (error.Exception is not null)
            _logger.Exception(error.Exception);
        _logger.Debug("Operation: {Operation}", context.Operation?.ToString());
        _logger.Debug("Variables: {Variables}", context.Variables?.ToString());
        base.SyntaxError(context, error);
    }

    public override void ValidationErrors(IRequestContext context, IReadOnlyList<IError> errors)
    {
        errors.ForEach(e =>
        {
            if (e.Exception is not null)
                _logger.Exception(e.Exception);
        });
        _logger.Debug("Operation: {Operation}", context.Operation?.ToString());
        _logger.Debug("Variables: {Variables}", context.Variables?.ToString());

        base.ValidationErrors(context, errors);
    }

    public override void RequestError(IRequestContext context, Exception exception)
    {
        _logger.Exception(exception);
        _logger.Debug("Operation: {Operation}", context.Operation?.ToString());
        _logger.Debug("Variables: {Variables}", context.Variables?.ToString());
        base.RequestError(context, exception);
    }

    public override void TaskError(IExecutionTask task, IError error)
    {
        if (error.Exception is not null)
            _logger.Exception(error.Exception);

        _logger.Warning("{Message}", error.Message);
        _logger.Debug("Task Kind: {Kind}", task.Kind.ToString());
        _logger.Debug("Path: {Path}", error.Path?.Print());
        _logger.Debug("Locations: {Locations}", error.Locations?.ToString());

        base.TaskError(task, error);
    }

    public override void ResolverError(IMiddlewareContext context, IError error)
    {
        if (error.Exception is not null)
            _logger.Exception(error.Exception);

        _logger.Warning("{Message}", error.Message);
        _logger.Debug("Operation: {Operation}", context.Operation?.ToString());
        _logger.Debug("Variables: {Variables}", context.Variables?.ToString());
        _logger.Debug("Path: {Path}", error.Path?.Print());
        _logger.Debug("Locations: {Locations}", error.Locations?.ToString());

        base.ResolverError(context, error);
    }
}