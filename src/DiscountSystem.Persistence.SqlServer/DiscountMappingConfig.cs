using DiscountSystem.Core.Domain.Entites;
using Mapster;

namespace DiscountSystem.Persistence.SqlServer;

public static class DiscountMappingConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<DiscountCodeTable, DiscountCode>
            .NewConfig()
            .Map(dest => dest.Code, src => src.Id)
            .Map(dest => dest.CreatedAtUtc, src => src.CreatedAtUtc)
            .Map(dest => dest.UsedAtUtc, src => src.UsedAtUtc)
            .Map(dest => dest.Status, src => src.Status);

        TypeAdapterConfig<DiscountCode, DiscountCodeTable>
            .NewConfig()
            .Map(dest => dest.Id, src => src.Code)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.CreatedAtUtc, src => src.CreatedAtUtc)
            .Map(dest => dest.UsedAtUtc, src => src.UsedAtUtc);
    }
}