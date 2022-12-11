using HotChocolate.AspNetCore.Instrumentation;
using Serilog;
using ILogger = Serilog.ILogger;
namespace Locker.Services;

public class ServerEventLogger : ServerDiagnosticEventListener
{
    private readonly ILogger _logger = Log.Logger.ForContext<ServerEventLogger>();

    public override void HttpRequestError(HttpContext context, Exception exception)
    {
        _logger.Exception(exception);
        _logger.Debug("Request: {Request}", context.ToString());
        _logger.Debug("Response: {Response}", context.Response.ToString());
        base.HttpRequestError(context, exception);
    }
}