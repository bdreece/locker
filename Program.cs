using Serilog;
using Serilog.Events;
using Locker;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
if (builder.Environment.IsDevelopment())
    builder.Configuration.AddJsonFile("appsettings.Development.json");
else if (builder.Environment.IsProduction())
    builder.Configuration.AddJsonFile("appsettings.Production.json");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.AddSerilog();

builder.AddLockerOptions()
    .AddLockerAuth()
    .AddLockerData()
    .AddLockerGraphQL();

var app = builder.Build();

app.UseAuthentication();

app.MapGraphQL();
app.MapBananaCakePop();

app.Run();
