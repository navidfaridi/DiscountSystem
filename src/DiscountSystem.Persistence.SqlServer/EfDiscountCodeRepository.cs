using DiscountSystem.Core.Application.Commands;
using DiscountSystem.Core.Application.Interfaces;
using DiscountSystem.Core.Application.Results;
using DiscountSystem.Core.Domain.Entites;
using DiscountSystem.Core.Domain.Enums;
using DiscountSystem.Core.Domain.ValueObjects;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace DiscountSystem.Persistence.SqlServer;


public class EfDiscountCodeRepository : IDiscountCodeRepository
{
    private readonly DiscountDbContext _db;

    private readonly IMapper _mapper;

    public EfDiscountCodeRepository(DiscountDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;

        _db.Database.EnsureCreated();
    }

    public async Task<bool> ExistsAsync(string code, CancellationToken ct = default)
    {
        var vo = new Code(code);
        return await _db.DiscountCodes.AsNoTracking()
            .AnyAsync(x => x.Id == vo, ct);
    }

    public async Task<int> AddRangeUniqueBatchAsync(IEnumerable<DiscountCode> codes, CancellationToken ct = default)
    {
        var voList = codes.Select(c => c.Code).ToList();

        var existing = await _db.DiscountCodes
            .AsNoTracking()
            .Where(x => voList.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(ct);

        var existingSet = new HashSet<Code>(existing);
        var toInsert = codes.Where(c => !existingSet.Contains(c.Code)).ToList();
        if (toInsert.Count == 0) return 0;

        var data = _mapper.Map<List<DiscountCodeTable>>(toInsert);
        _db.DiscountCodes.AddRange(data);

        var affected = await _db.SaveChangesAsync(ct);
        return affected;
    }

    public async Task<DiscountCode?> GetAsync(string code, CancellationToken ct = default)
    {
        var vo = new Code(code);
        var data = await _db.DiscountCodes.FirstOrDefaultAsync(x => x.Id == vo, ct);

        return data is null ? null : _mapper.Map<DiscountCode>(data);
    }
    public async Task UpdateAsync(DiscountCode code, CancellationToken ct = default)
    {
        var data = _mapper.Map<DiscountCodeTable>(code);
        _db.DiscountCodes.Update(data);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> TryConsumeAsync(string code, DateTime usedAtUtc, CancellationToken ct = default)
    {
        var vo = new Code(code);
        var affected = await _db.DiscountCodes
            .Where(x => x.Id == vo && x.Status == DiscountStatus.Available)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.Status, DiscountStatus.Used)
                .SetProperty(x => x.UsedAtUtc, usedAtUtc), ct);

        return affected == 1;
    }

    public async Task<DiscountStatus?> GetStatusAsync(string code, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var vo = new Code(code);

        return await _db.DiscountCodes
            .AsNoTracking()
            .Where(x => x.Id == vo)
            .Select(x => (DiscountStatus?)x.Status)
            .FirstOrDefaultAsync(ct);
    }
}