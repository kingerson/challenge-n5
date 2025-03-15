namespace MsClean.Infrastructure;
public interface IMemoryCacheService
{
    bool TryGetValue<T>(string key, out T value);
    void SetValue<T>(string key, T value);
    void Remove(string key);
}
