using DiscountSystem.Core.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace DiscountSystem.RedisCache;

public class RedisCacheService : ICacheService, IAsyncDisposable
{
    private readonly string _connectionString;
    private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
    private IDatabase Database => _lazyConnection.Value.GetDatabase();
    public RedisCacheService(string connectionString)
    {
        _connectionString = connectionString;
        _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_connectionString));
    }
    public async ValueTask DisposeAsync()
    {
        if (_lazyConnection.IsValueCreated)
            await _lazyConnection.Value.CloseAsync();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        var value = await Database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch
        {
            await RemoveAsync(key, ct);
            return default;
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        await Database.KeyDeleteAsync(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));

        var json = JsonSerializer.Serialize(value);
        await Database.StringSetAsync(key, json, expiration);
    }
}
