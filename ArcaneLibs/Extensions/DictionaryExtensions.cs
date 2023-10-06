using System.IO.Compression;
using System.Text;

namespace ArcaneLibs.Extensions;

public static class DictionaryExtensions {
    public static bool ChangeKey<TKey, TValue>(this IDictionary<TKey, TValue> dict,
        TKey oldKey, TKey newKey) {
        TValue value;
        if (!dict.Remove(oldKey, out value))
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
        lock (dict)
            dict.TryAdd(key, value);
        return value;
    }

    public static async Task<Y> GetOrCreateAsync<X, Y>(this IDictionary<X, Y> dict, X key, Func<X, Task<Y>> valueFactory, SemaphoreSlim? semaphore = null) {
        if (semaphore is not null) await semaphore.WaitAsync();
        if (dict.TryGetValue(key, out var value)) {
            if (semaphore is not null) semaphore.Release();
            return value;
        }

        value = await valueFactory(key);
        dict.Add(key, value);
        if (semaphore is not null) semaphore.Release();
        return value;
    }

    public static bool StartsWith<T>(this IEnumerable<T> list, IEnumerable<T> prefix) {
        return prefix.SequenceEqual(list.Take(prefix.Count()));
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
        using ZLibStream stream = new ZLibStream(inStream, CompressionMode.Decompress);
        using var result = new MemoryStream();
        stream.CopyTo(result);
        stream.Flush();
        stream.Close();
        return result.ToArray();
    }
}
