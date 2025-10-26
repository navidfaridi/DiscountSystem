using DiscountSystem.Core.Application.Commands;
using DiscountSystem.Core.Application.Interfaces;
using DiscountSystem.Core.Application.Results;
using DiscountSystem.Core.Domain.Entites;
using DiscountSystem.Core.Domain.Enums;
using DiscountSystem.Core.Domain.ValueObjects;
using FluentValidation;

namespace DiscountSystem.Services.Services;

public class DiscountAppService : IDiscountAppService
{
    private readonly IDiscountCodeRepository _repo;
    private readonly IRandomGenerator _rng;
    private readonly IClock _clock;
    private readonly IValidator<GenerateCodesCommand> _generateValidator;
    private readonly IValidator<UseCodeCommand> _useValidator;
    private readonly ICacheService _cache;

    private const int ChunkSize = 100;
    private const int MaxAttempts = 50;
    private const string UsedCachePrefix = "usedcode:";
    private static readonly TimeSpan UsedCacheTtl = TimeSpan.FromHours(6);

    public DiscountAppService(IDiscountCodeRepository repo,
                              IRandomGenerator rng,
                              IClock clock,
                              IValidator<UseCodeCommand> useValidator,
                              IValidator<GenerateCodesCommand> generateValidator,
                              ICacheService cache)
    {
        _repo = repo; _rng = rng; _clock = clock;
        _useValidator = useValidator;
        _generateValidator = generateValidator;
        _cache = cache;
    }

    public async Task<GenerateCodesResult> GenerateAsync(GenerateCodesCommand cmd, CancellationToken ct)
    {
        await _generateValidator.ValidateAndThrowAsync(cmd, ct);

        int remaining = cmd.Count;
        int attempts = 0;
        var now = _clock.UtcNow;

        while (remaining > 0 && attempts++ < MaxAttempts)
        {
            int toGenerate = Math.Min(remaining, ChunkSize);

            var uniq = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            while (uniq.Count < toGenerate)
                uniq.Add(_rng.NextCode(cmd.Length));

            var batch = uniq.Select(c => new DiscountCode(new Code(c), now)).ToList();

            int inserted = await _repo.AddRangeUniqueBatchAsync(batch, ct);
            remaining -= inserted;
        }

        return new GenerateCodesResult(remaining == 0);
    }

    public async Task<UseCodeResult> UseAsync(UseCodeCommand cmd, CancellationToken ct)
    {
        await _useValidator.ValidateAndThrowAsync(cmd, ct);
        var code = new SimpleDiscountCode
        {
            Code = cmd.Code.Trim().ToUpperInvariant()
        };

        var udc = await _cache.GetAsync<SimpleDiscountCode>($"{UsedCachePrefix}:{code.Code}", ct);
        if (udc is not null)
        {
            return udc.Status;
        }

        var result = await _repo.TryConsumeAsync(cmd.Code, _clock.UtcNow, ct);
        if (result)
        {
            code.Status = UseCodeResult.AlreadyUsed;
            await _cache.SetAsync($"{UsedCachePrefix}:{code}", code, UsedCacheTtl, ct);
            return UseCodeResult.Success;
        }

        var existing = await _repo.GetStatusAsync(cmd.Code, ct);
        if (existing is null)
        {
            code.Status = UseCodeResult.NotFound;
            await _cache.SetAsync($"{UsedCachePrefix}:{code}", code, TimeSpan.FromMinutes(30), ct);
            return UseCodeResult.NotFound;
        }

        if (existing == DiscountStatus.Used)
        {
            code.Status = UseCodeResult.AlreadyUsed;
            await _cache.SetAsync($"{UsedCachePrefix}:{code}", code, UsedCacheTtl, ct);
            return UseCodeResult.AlreadyUsed;
        }

        return UseCodeResult.UnknownError;
    }
}