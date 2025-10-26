using DiscountSystem.Core.Domain.Entites;
using DiscountSystem.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscountSystem.Core.Application.Interfaces;

public interface IDiscountCodeRepository
{
    Task<bool> ExistsAsync(string code, CancellationToken ct = default);
    Task<int> AddRangeUniqueBatchAsync(IEnumerable<DiscountCode> codes, CancellationToken ct = default);
    Task<DiscountCode?> GetAsync(string code, CancellationToken ct = default);
    Task<DiscountStatus?> GetStatusAsync(string code, CancellationToken ct = default);
    Task UpdateAsync(DiscountCode code, CancellationToken ct = default);
    Task<bool> TryConsumeAsync(string code, DateTime usedAtUtc, CancellationToken ct = default);
}
