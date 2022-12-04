using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

using Locker;
using Locker.Models.Entities;
using Locker.Resolvers;
using Locker.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.AddSerilog();

builder.Services.AddOptions<LockerOptions>()
    .Configure<IConfiguration>((options, config) =>
    {
        config.GetSection("Locker").Bind(options);
    });

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var section = builder.Configuration.GetSection("Locker");
        var issuer = section.GetValue<string>("Issuer");
        var secret = section.GetValue<string>("Secret");
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secret ?? "secret"));

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                IssuerSigningKey = signingKey
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddTransient<ITokenService, TokenService>();

builder.Services.AddPooledDbContextFactory<DataContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("lockerdb");
    options.UseNpgsql(connection, o =>
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});

builder.Services.AddGraphQLServer()
    .RegisterDbContext<DataContext>(DbContextKind.Pooled)
    .AddAuthorization()
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
