using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Text;

namespace ArcaneLibs.Extensions;

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API: extension methods")]
public static class DictionaryExtensions {
    public static void RemoveAll<K, V>(this IDictionary<K, V> dict, Func<K, V, bool> match) {
        foreach (var key in dict.Keys.ToArray()
                     .Where(key => match(key, dict[key])))
            dict.Remove(key);
    }

    public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey oldKey, TKey newKey) {
        if (!dict.Remove(oldKey, out var value))
            return false;

        dict[newKey] = value; // or dict.Add(newKey, value) depending on ur comfort
        return true;
    }

    public static Y GetOrCreate<X, Y>(this IDictionary<X, Y> dict, X key) where Y : new() {
        if (dict.TryGetValue(key, out var value)) return value;

        value = new Y();
        dict.Add(key, value);
        return value;
    }

    public static Y GetOrCreate<X, Y>(this IDictionary<X, Y> dict, X key, Func<X, Y> valueFactory) {
        if (dict.TryGetValue(key, out var value)) return value;

        value = valueFactory(key);
        lock (dict) {
            dict.TryAdd(key, value);
        }

        return value;
    }

    public static async Task<TY?> GetOrCreateAsync<TX, TY>(this IDictionary<TX, TY> dict, TX key, Func<TX, Task<TY>> valueFactory, SemaphoreSlim? semaphore = null) {
        if (semaphore is not null) await semaphore.WaitAsync();
        // lock (dict) {
        if (dict.TryGetValue(key, out var value)) {
            semaphore?.Release();
            return value;
        }

        value = await valueFactory(key);
        dict.TryAdd(key, value);
        // }

        semaphore?.Release();
        return value;
    }

    public static bool StartsWith<T>(this IEnumerable<T> list, IEnumerable<T> prefix) {
        var array = prefix as T[] ?? prefix.ToArray();
        var prefixLen = array.Length;
        return array.SequenceEqual(list.Take(prefixLen));
    }

    public static void HexDump(this IEnumerable<byte> bytes, int width = 32) {
        var data = new Queue<(string hex, char utf8)>(bytes.ToArray().Select(x => ($"{x:X2}", (char)x)).ToArray());
        while (data.Count > 0) {
            var line = data.Dequeue(Math.Min(width, data.Count)).ToArray();
            Console.WriteLine(
                string.Join(" ", line.Select(x => x.hex)).PadRight(width * 3)
                + " | "
                + string.Join("", line.Select(x => x.utf8))
                    .Replace('\n', '.')
                    .Replace('\r', '.')
                    .Replace('\0', '.')
                    .Replace('\t', '.')
                    .Replace('\v', '.')
                    .Replace('\b', '.')
                    .Replace('\a', '.')
                    .Replace('\f', '.')
            );
        }
    }

    public static string AsHexString(this IEnumerable<byte> bytes) => string.Join(' ', bytes.Select(x => $"{x:X2}"));
    public static string AsString(this IEnumerable<byte> bytes) => Encoding.UTF8.GetString(bytes.ToArray());

    //zlib decompress
    public static byte[] ZlibDecompress(this IEnumerable<byte> bytes) {
        var inStream = new MemoryStream(bytes.ToArray());
        using var stream = new ZLibStream(inStream, CompressionMode.Decompress);
        using var result = new MemoryStream();
        stream.CopyTo(result);
        stream.Flush();
        stream.Close();
        return result.ToArray();
    }

    public static T GetByCaseInsensitiveKey<T>(this IDictionary<string, T> dict, string key) => dict.First(x => x.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase)).Value;
}