namespace ArcaneLibs.Collections;

public class LruCache<T>(int maxItems) {
    private readonly Dictionary<string, CacheItem> _items = new();

    public async Task<T?> GetOrAddAsync(string key, Func<Task<T>> factory) {
        if (_items.TryGetValue(key, out var cacheItem)) {
            cacheItem.LastAccessed = DateTimeOffset.UtcNow;
            return cacheItem.Value;
        }

        var value = await factory();
        _items[key] = new CacheItem {
            Value = value,
            LastAccessed = DateTimeOffset.UtcNow
        };

        if (_items.Count > maxItems) {
            var oldestKey = _items.OrderBy(kv => kv.Value.LastAccessed).First().Key;
            _items.Remove(oldestKey);
        }

        return value;
    }
    
    public int ExpireAllOlderThan(TimeSpan age) {
        var now = DateTimeOffset.UtcNow;
        var keysToRemove = _items.Where(kv => now - kv.Value.LastAccessed > age).Select(kv => kv.Key).ToList();
        foreach (var key in keysToRemove) {
            _items.Remove(key);
        }
        return keysToRemove.Count;
    }
    
    public int ExpireAll(Func<string, T, bool> predicate) {
        var keysToRemove = _items.Where(kv => predicate(kv.Key, kv.Value.Value)).Select(kv => kv.Key).ToList();
        foreach (var key in keysToRemove) {
            _items.Remove(key);
        }
        return keysToRemove.Count;
    }

    private class CacheItem {
        public required T Value { get; set; }
        public DateTimeOffset LastAccessed { get; set; }
    }
}