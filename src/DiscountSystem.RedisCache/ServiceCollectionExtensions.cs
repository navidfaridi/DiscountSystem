using DiscountSystem.Core.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DiscountSystem.RedisCache;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, string connStr)
    {
        services.AddSingleton<ICacheService>(new RedisCacheService(connStr));
        return services;
    }
}
