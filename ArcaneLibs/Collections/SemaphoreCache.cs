using System.Collections.Concurrent;

namespace ArcaneLibs.Collections;

public class SemaphoreCache<T> {
    private readonly ConcurrentDictionary<string, T> _values = new();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();
    public bool StoreNulls { get; set; } = false;

    public async Task<T> GetOrAdd(string key, Func<Task<T>> factory) {
        ArgumentNullException.ThrowIfNull(key);
        _semaphores.TryAdd(key, new SemaphoreSlim(1, 1));
        await _semaphores[key].WaitAsync();
        if (_values.TryGetValue(key, out var value)) {
            _semaphores[key].Release();
            return value;
        }

        try {
            var val = await factory();
            _values.TryAdd(key, val);
            return val;
        }
        finally {
            _semaphores[key].Release();
        }
    }
    
    public async Task<T?> TryGetOrAdd(string key, Func<Task<T>> factory) {
        try {
            return await GetOrAdd(key, factory);
        }
        catch {
            return default;
        }
    }
}

public class ExpiringSemaphoreCache<T> {
    private readonly ConcurrentDictionary<string, T> _values = new();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();
    private readonly ConcurrentDictionary<string, DateTime> _expiry = new();

    public async Task<T> GetOrAdd(string key, Func<Task<T>> factory, TimeSpan expiry) {
        ArgumentNullException.ThrowIfNull(key);
        _semaphores.TryAdd(key, new SemaphoreSlim(1, 1));
        await _semaphores[key].WaitAsync();
        if (_values.TryGetValue(key, out var value) && _expiry.TryGetValue(key, out var exp) && exp > DateTime.Now) {
            _semaphores[key].Release();
            return value;
        }

        var val = await factory();
        _values.TryAdd(key, val);
        _expiry.TryAdd(key, DateTime.Now + expiry);
        _semaphores[key].Release();

        _ = Task.Delay(expiry).ContinueWith(__ => {
            _values.TryRemove(key, out _);
            _semaphores.TryRemove(key, out _);
        });

        return val;
    }
}