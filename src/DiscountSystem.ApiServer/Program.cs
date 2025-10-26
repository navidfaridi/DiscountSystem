using DiscountSystem.ApiServer.Middlewares;
using DiscountSystem.Persistence.SqlServer;
using DiscountSystem.Services.Services;
using DiscountSystem.RedisCache;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDiscountSystemCore();

builder.Services.AddControllers();

var connStr = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddRedisCache(connStr);

connStr = builder.Configuration.GetConnectionString("Default") ?? "Server=.;Database=DiscountSystemDb;Encrypt=True;TrustServerCertificate=True;uid=sa;pwd=N@vid!@#;";
builder.Services.AddDiscountSystemSqlServer(connStr);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.MapControllers();

app.Run();
