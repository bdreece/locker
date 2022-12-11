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