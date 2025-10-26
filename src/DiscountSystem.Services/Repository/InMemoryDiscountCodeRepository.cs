using DiscountSystem.Core.Application.Interfaces;
using DiscountSystem.Core.Domain.Entites;
using DiscountSystem.Core.Domain.Enums;
using System.Collections.Concurrent;

namespace DiscountSystem.Services.Repository;

public class InMemoryDiscountCodeRepository : IDiscountCodeRepository
{
    private readonly ConcurrentDictionary<string, DiscountCode> _dict = new(StringComparer.OrdinalIgnoreCase);

    public Task<bool> ExistsAsync(string code, CancellationToken ct = default)
        => Task.FromResult(_dict.ContainsKey(code));

    public Task AddRangeAsync(IEnumerable<DiscountCode> codes, CancellationToken ct = default)
    {
        foreach (var c in codes) _dict[c.Code.Value] = c;
        return Task.CompletedTask;
    }

    public Task<DiscountCode?> GetAsync(string code, CancellationToken ct = default)
    {
        _dict.TryGetValue(code, out var entity);
        return Task.FromResult(entity);
    }

    public Task UpdateAsync(DiscountCode code, CancellationToken ct = default)
    {
        _dict[code.Code.Value] = code;
        return Task.CompletedTask;
    }

    public async Task<bool> TryConsumeAsync(string code, DateTime usedAtUtc, CancellationToken ct = default)
    {
        var item = await this.GetAsync(code, ct);
        if (item is null || item.Status != Core.Domain.Enums.DiscountStatus.Available)
            return false;

        item.Use(usedAtUtc);
        await this.UpdateAsync(item, ct);
        return true;
    }

    public Task<int> AddRangeUniqueBatchAsync(IEnumerable<DiscountCode> codes, CancellationToken ct = default)
    {
        foreach (var c in codes) _dict[c.Code.Value] = c;
        return Task.FromResult(codes.Count());
    }

    public async Task<DiscountStatus?> GetStatusAsync(string code, CancellationToken ct = default)
    {
        return await this.GetAsync(code, ct) is DiscountCode dc ? dc.Status : null;
    }
}
