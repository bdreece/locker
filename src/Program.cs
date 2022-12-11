using Locker;

var builder = WebApplication.CreateBuilder(args);

builder.AddLockerOptions()
    .AddLockerLogging()
    .AddLockerAuth()
    .AddLockerData()
    .AddLockerGraphQL();

var app = builder.Build();

app.UseAuthentication();
app.MapGraphQL();
app.MapBananaCakePop();

app.Run();
