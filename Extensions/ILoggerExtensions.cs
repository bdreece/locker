using ILogger = Serilog.ILogger;

namespace Locker;

public static class ILoggerExtensions
{
    public static void Exception(this ILogger logger, Exception e)
    {
        logger.Error(e.Message);

        logger.Warning("Caught exception {ExceptionType}!", e.GetType().Name);
        logger.Verbose("{@Exception}", e);
    }
}