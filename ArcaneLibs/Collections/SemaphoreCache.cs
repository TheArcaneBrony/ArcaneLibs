using System.Collections.Concurrent;

namespace ArcaneLibs.Collections;

public class SemaphoreCache<T> where T : class {
    
    private readonly ConcurrentDictionary<string, T> _values = new();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();
    
    public async Task<T> GetOrAdd(string key, Func<Task<T>> factory) {
        ArgumentNullException.ThrowIfNull(key);
        _semaphores.TryAdd(key, new SemaphoreSlim(1, 1));
        await _semaphores[key].WaitAsync();
        if (_values.TryGetValue(key, out var value)) {
            _semaphores[key].Release();
            return value;
        }

        var val = await factory();
        _values.TryAdd(key, val);
        _semaphores[key].Release();
        return val;
    }
}

public class ExiringSemaphoreCache<T> where T : class {
    
    private readonly ConcurrentDictionary<string, T> _values = new();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();
    
    public async Task<T> GetOrAdd(string key, Func<Task<T>> factory, TimeSpan expiry) {
        ArgumentNullException.ThrowIfNull(key);
        _semaphores.TryAdd(key, new SemaphoreSlim(1, 1));
        await _semaphores[key].WaitAsync();
        if (_values.TryGetValue(key, out var value)) {
            _semaphores[key].Release();
            return value;
        }

        var val = await factory();
        _values.TryAdd(key, val);
        _semaphores[key].Release();
        
#pragma warning disable CS4014 // We intentionally do not await this task - this handles cache expiry
        Task.Delay(expiry).ContinueWith(__ => {
            _values.TryRemove(key, out var _);
            _semaphores.TryRemove(key, out var _);
        });
#pragma warning restore CS4014
        
        return val;
    }
}