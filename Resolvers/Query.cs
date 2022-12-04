using Serilog;
using ILogger = Serilog.ILogger;

namespace Locker.Resolvers;

public partial class Query
{
    private static readonly ILogger _logger = Log.Logger.ForContext<Query>();
}