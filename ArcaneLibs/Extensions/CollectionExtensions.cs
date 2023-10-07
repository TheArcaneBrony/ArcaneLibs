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

    //return task results async without preserving order
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<Task<T>> tasks) {
        var taskList = tasks.ToList();
        while (taskList.Count > 0) {
            var task = await Task.WhenAny(taskList);
            taskList.Remove(task);
            yield return await task;
        }
    }

    //return task results async without preserving order
    public static async IAsyncEnumerable<(Task task, T result)> ToAsyncEnumerableWithContext<T>(this IEnumerable<Task<T>> tasks) {
        var taskList = tasks.ToList();
        while (taskList.Count > 0) {
            var task = await Task.WhenAny(taskList);
            taskList.Remove(task);
            yield return (task, await task);
        }
    }
}
