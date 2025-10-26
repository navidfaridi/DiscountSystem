using DiscountSystem.Core.Application.Interfaces;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DiscountSystem.Persistence.SqlServer;

public static class SqlServerServiceCollectionExtensions
{
    public static IServiceCollection AddDiscountSystemSqlServer(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContextPool<DiscountDbContext>(db =>
        {
            db.UseSqlServer(connectionString);
        });

        DiscountMappingConfig.Configure();
        services.AddSingleton(TypeAdapterConfig.GlobalSettings);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddScoped<IDiscountCodeRepository, EfDiscountCodeRepository>();

        return services;
    }
}