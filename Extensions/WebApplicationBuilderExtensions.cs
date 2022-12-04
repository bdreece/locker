using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

using Locker.Models.Entities;
using Locker.Resolvers;
using Locker.Services;

namespace Locker;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddLockerOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddOptions<LockerOptions>()
            .Configure<IConfiguration>((options, config) =>
            {
                config.GetSection("Locker").Bind(options);
            });

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
        return builder;
    }

    public static WebApplicationBuilder AddLockerData(this WebApplicationBuilder builder)
    {
        builder.Services.AddPooledDbContextFactory<DataContext>(options =>
        {
            var connection = builder.Configuration.GetConnectionString("lockerdb");
            options.UseNpgsql(connection, o =>
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });
        return builder;
    }

    public static WebApplicationBuilder AddLockerGraphQL(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddGraphQLServer()
            .AddDiagnosticEventListener<ExecutionEventLogger>()
            .AddDiagnosticEventListener<ServerEventLogger>()
            .RegisterDbContext<DataContext>(DbContextKind.Pooled)
            .AddAuthorization()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .AddQueryType<Query>()
            .AddMutationType<Mutation>();

        return builder;
    }
}