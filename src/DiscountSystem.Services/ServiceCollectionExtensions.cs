using DiscountSystem.Core.Application.Commands;
using DiscountSystem.Core.Application.Interfaces;
using DiscountSystem.Core.Application.Validators;
using DiscountSystem.Services.Repository;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DiscountSystem.Services.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscountSystemCore(this IServiceCollection services)
    {
        services.AddScoped<IValidator<GenerateCodesCommand>, GenerateCodesCommandValidator>();
        services.AddScoped<IValidator<UseCodeCommand>, UseCodeCommandValidator>();
        services.TryAddSingleton<IClock, SystemClock>();
        services.TryAddSingleton<IRandomGenerator, SecureRandomGenerator>();
        services.TryAddScoped<IDiscountAppService, DiscountAppService>();
        return services;
    }

    public static IServiceCollection AddDiscountSystemInMemoryStorage(this IServiceCollection services)
    {
        services.AddSingleton<IDiscountCodeRepository, InMemoryDiscountCodeRepository>();
        return services;
    }
}
