namespace MsClean.Infrastructure;
using System;
using Microsoft.Extensions.Caching.Memory;

public class MemoryCacheService : IMemoryCacheService
{
    private static readonly int _SIZELIMIT = 1024;
    private static readonly int _ABSOLUTETIME = 90;
    private static readonly int _SLIDINGTIME = 90;
    private readonly IMemoryCache _memoryCache;
    public MemoryCacheService(IMemoryCache memoryCache) => _memoryCache = memoryCache;
    public void Remove(string key) => _memoryCache.Remove(key);

    public void SetValue<T>(string key, T value)
    {
        _memoryCache.Set(key, value, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(_ABSOLUTETIME),
            SlidingExpiration = TimeSpan.FromMinutes(_SLIDINGTIME),
            Size = _SIZELIMIT
        });
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        value = default;
        if (_memoryCache.TryGetValue(key, out T result))
        {
            value = result;
            return true;
        }

        return false;
    }
}
