using DiscountSystem.Persistence.SqlServer;
using DiscountSystem.Services.Services;
using DiscountSystem.SignalRServer;
using Microsoft.AspNetCore.SignalR;
using DiscountSystem.RedisCache;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR(o =>
{
    o.EnableDetailedErrors = builder.Environment.IsDevelopment();
})
    .AddMessagePackProtocol();
builder.Services.AddSingleton<IHubFilter, HubErrorFilter>();

var connStr = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddRedisCache(connStr);

connStr = builder.Configuration.GetConnectionString("Default") ?? "Server=.;Database=DiscountSystemDb;Encrypt=True;TrustServerCertificate=True;uid=sa;pwd=N@vid!@#;";
builder.Services.AddDiscountSystemSqlServer(connStr);
builder.Services.AddDiscountSystemCore();

var app = builder.Build();

app.MapHub<DiscountHub>("/discount");

app.Run();
