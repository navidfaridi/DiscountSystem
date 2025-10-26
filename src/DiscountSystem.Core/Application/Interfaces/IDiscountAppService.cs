using DiscountSystem.Core.Application.Commands;
using DiscountSystem.Core.Application.Results;
using DiscountSystem.Core.Domain.Entites;
using DiscountSystem.Core.Domain.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscountSystem.Core.Application.Interfaces;

public interface IDiscountAppService
{
    Task<GenerateCodesResult> GenerateAsync(GenerateCodesCommand cmd, CancellationToken ct);

    Task<UseCodeResult> UseAsync(UseCodeCommand cmd, CancellationToken ct);
}
