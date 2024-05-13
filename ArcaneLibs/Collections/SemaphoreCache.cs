using System.Collections.Concurrent;

namespace ArcaneLibs.Collections;

public class SemaphoreCache<T> where T : class {
    
    internal readonly ConcurrentDictionary<string, T> Values = new();
    internal readonly ConcurrentDictionary<string, SemaphoreSlim> Semaphores = new();
    
    public async Task<T> GetOrAdd(string key, Func<Task<T>> factory) {
        ArgumentNullException.ThrowIfNull(key);
        Semaphores.TryAdd(key, new SemaphoreSlim(1, 1));
        await Semaphores[key].WaitAsync();
        if (Values.TryGetValue(key, out var value)) {
            Semaphores[key].Release();
            return value;
        }

        var val = await factory();
        Values.TryAdd(key, val);
        Semaphores[key].Release();
        return val;
    }
}

public class ExpiringSemaphoreCache<T> : SemaphoreCache<T> where T : class  {
    private readonly ConcurrentDictionary<string, DateTime> _expiry = new();
    
    public async Task<T> GetOrAdd(string key, Func<Task<T>> factory, TimeSpan expiry) {
        ArgumentNullException.ThrowIfNull(key);
        Semaphores.TryAdd(key, new SemaphoreSlim(1, 1));
        await Semaphores[key].WaitAsync();
        if (Values.TryGetValue(key, out var value) && _expiry.TryGetValue(key, out var exp) && exp > DateTime.Now) {
            Semaphores[key].Release();
            return value;
        }

        var val = await factory();
        Values.TryAdd(key, val);
        _expiry.TryAdd(key, DateTime.Now + expiry);
        Semaphores[key].Release();
        return val;
    }
}