using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using SendGrid.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Compact;

using Locker.Models.Entities;
using Locker.Resolvers;
using Locker.Services;

namespace Locker;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddLockerOptions(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddJsonFile("appsettings.json");
        if (builder.Environment.IsDevelopment())
            builder.Configuration.AddJsonFile("appsettings.Development.json");
        else if (builder.Environment.IsProduction())
            builder.Configuration.AddJsonFile("appsettings.Production.json");

        builder.Services.AddOptions<LockerOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                config.GetSection("Locker").Bind(options);
            });

        return builder;
    }

    public static WebApplicationBuilder AddLockerLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                new CompactJsonFormatter(),
                "logs/log.clef",
                rollingInterval: RollingInterval.Day
            )
            .CreateLogger();

        builder.Logging.AddSerilog();
        return builder;
    }

    public static WebApplicationBuilder AddLockerAuth(this WebApplicationBuilder builder)
    {

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var section = builder.Configuration.GetSection("Locker");
                var issuer = section.GetValue<string>("Issuer");
                var secret = section.GetValue<string>("SecretKey");
                var signingKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secret ?? "secret"));

                options.IncludeErrorDetails = true;
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateAudience = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        IssuerSigningKey = signingKey
                    };
            });

        builder.Services.AddAuthorization();
        builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
        builder.Services.AddTransient<ITokenService, TokenService>();
        return builder;
    }

    public static WebApplicationBuilder AddLockerData(this WebApplicationBuilder builder)
    {
        builder.Services.AddPooledDbContextFactory<DataContext>(options =>
        {
            var connection = builder.Configuration
                .GetConnectionString("lockerdb");
            options.UseNpgsql(connection, o =>
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });
        return builder;
    }

    public static WebApplicationBuilder AddLockerEmail(this WebApplicationBuilder builder)
    {
        builder.Services.AddSendGrid(options =>
        {
            var apiKey = builder.Configuration
                .GetSection("Locker")
                .GetValue<string>("SendGridApiKey");
            options.ApiKey = apiKey;
        });

        return builder;
    }

    public static WebApplicationBuilder AddLockerGraphQL(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddHttpContextAccessor()
            .AddGraphQLServer()
            .RegisterDbContext<DataContext>(DbContextKind.Pooled)
            .AddDiagnosticEventListener<ExecutionEventLogger>()
            .AddDiagnosticEventListener<ServerEventLogger>()
            .AddHttpRequestInterceptor<HttpRequestInterceptor>()
            .AddGlobalObjectIdentification()
            .AddAuthorization()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>()
            .AddMutationConventions(applyToAllMutations: true)
            .AddQueryFieldToMutationPayloads();

        return builder;
    }
}