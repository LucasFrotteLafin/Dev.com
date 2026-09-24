using System.Text.Json;
using DevCom.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace DevCom.Infrastructure.Cache;

public class RedisCacheService(
    IDistributedCache      cache,
    IConnectionMultiplexer redis,
    ILogger<RedisCacheService> logger) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
    {
        try
        {
            var bytes = await cache.GetAsync(key, ct);
            if (bytes is null || bytes.Length == 0) return null;
            return JsonSerializer.Deserialize<T>(bytes, JsonOpts);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache GET falhou para chave {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
        where T : class
    {
        try
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(value, JsonOpts);
            var opts  = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5)
            };
            await cache.SetAsync(key, bytes, opts, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache SET falhou para chave {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        try { await cache.RemoveAsync(key, ct); }
        catch (Exception ex) { logger.LogWarning(ex, "Cache REMOVE falhou para chave {Key}", key); }
    }

    // Fix 9: GetServer movido para dentro do try — evita RedisConnectionException nao capturada
    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        try
        {
            var endPoints = redis.GetEndPoints();
            if (endPoints.Length == 0) return;

            var server = redis.GetServer(endPoints[0]);
            if (!server.IsConnected) return;

            var db = redis.GetDatabase();

            await foreach (var key in server.KeysAsync(pattern: $"{prefix}*", pageSize: 250))
            {
                ct.ThrowIfCancellationRequested();
                await db.KeyDeleteAsync(key);
            }
        }
        catch (OperationCanceledException) { throw; }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache REMOVE_PREFIX falhou para prefixo {Prefix}", prefix);
        }
    }
}