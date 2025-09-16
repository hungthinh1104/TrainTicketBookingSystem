using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using TrainTicketBookingSystem.Application.Interfaces;

namespace TrainTicketBookingSystem.Infrastructure.Caching;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IConnectionMultiplexer? _redis;
    private readonly IDatabase? _database;
    private readonly ILogger<CacheService> _logger;

    public CacheService(
        IMemoryCache memoryCache,
        IConnectionMultiplexer? redis,
        ILogger<CacheService> logger)
    {
        _memoryCache = memoryCache;
        _redis = redis;
        _database = redis?.GetDatabase();
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            // Try memory cache first
            if (_memoryCache.TryGetValue(key, out T? memoryValue))
            {
                return memoryValue;
            }

            // Try Redis if available
            if (_database != null)
            {
                var redisValue = await _database.StringGetAsync(key);
                if (redisValue.HasValue)
                {
                    var deserializedValue = JsonSerializer.Deserialize<T>(redisValue!);
                    
                    // Cache in memory for faster subsequent access
                    _memoryCache.Set(key, deserializedValue, TimeSpan.FromMinutes(5));
                    
                    return deserializedValue;
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cached value for key {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var defaultExpiration = expiration ?? TimeSpan.FromMinutes(30);

            // Set in memory cache
            _memoryCache.Set(key, value, defaultExpiration);

            // Set in Redis if available
            if (_database != null && value != null)
            {
                var serializedValue = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(key, serializedValue, defaultExpiration);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cached value for key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            // Remove from memory cache
            _memoryCache.Remove(key);

            // Remove from Redis if available
            if (_database != null)
            {
                await _database.KeyDeleteAsync(key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cached value for key {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            if (_redis != null)
            {
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var keys = server.Keys(pattern: pattern);
                
                if (_database != null)
                {
                    await _database.KeyDeleteAsync(keys.ToArray());
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cached values by pattern {Pattern}", pattern);
        }
    }
}