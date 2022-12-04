using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

using Locker.Resolvers;
using Locker.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.AddSerilog();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("MySuperSecretKey"));

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidIssuer = "https://locker.bdreece.dev",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey
            };
    });

builder.Services.AddPooledDbContextFactory<DataContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("lockerdb");
    options.UseNpgsql(connection, o =>
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});

builder.Services.AddGraphQLServer()
    .RegisterDbContext<DataContext>(DbContextKind.Pooled)
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>();

var app = builder.Build();

app.UseAuthentication();
app.UseHttpsRedirection();

app.MapGraphQL();
app.MapBananaCakePop();

app.Run();
