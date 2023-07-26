namespace ArcaneLibs.Extensions;

public static class CollectionExtensions {
    public static void RemoveAll<K, V>(this IDictionary<K, V> dict, Func<K, V, bool> match) {
        foreach (var key in dict.Keys.ToArray()
                     .Where(key => match(key, dict[key])))
            dict.Remove(key);
    }

    public static T[] Add<T>(this T[] array, T item) {
        Array.Resize(ref array, array.Length + 1);
        array[^1] = item;
        return array;
    }
}