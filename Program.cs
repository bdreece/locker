using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Locker.Resolvers;
using Locker.Services;

var builder = WebApplication.CreateBuilder(args);

var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes("MySuperSecretKey"));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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
    options.UseNpgsql(connection, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
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
