using HotChocolate.Execution;
using HotChocolate.Execution.Instrumentation;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Locker.Services;

public class ExecutionEventLogger : ExecutionDiagnosticEventListener
{
    private readonly ILogger _logger = Log.Logger.ForContext<ExecutionEventLogger>();

    public override void RequestError(IRequestContext context, Exception exception)
    {
        _logger.Exception(exception);
        _logger.Debug("Operation: {Operation}", context.Operation?.ToString());
        _logger.Debug("Variables: {Variables}", context.Variables?.ToString());
        base.RequestError(context, exception);
    }
}