namespace ArcaneLibs.Extensions;

public static class CollectionExtensions {
    public static T[] Add<T>(this T[] array, T item) {
        Array.Resize(ref array, array.Length + 1);
        array[^1] = item;
        return array;
    }

    //return task results async without preserving order
    [Obsolete("Replaced by Task.WhenEach in .NET 9")]
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<Task<T>> tasks, bool skipExceptions = false) {
        var taskList = tasks.ToList();
        while (taskList.Count > 0) {
            var task = await Task.WhenAny(taskList);
            taskList.Remove(task);
            if (skipExceptions && task.IsFaulted) continue;
            yield return await task;
        }
    }

    public static int GetWidth<T>(this T[,] array) => array.GetLength(1);
    public static int GetHeight<T>(this T[,] array) => array.GetLength(0);

    public static void MergeBy<T>(this List<T> list, IEnumerable<T> other, Func<T, T, bool> predicate, Action<T, T> mergeAction) {
        foreach (var item in other) {
            var existing = list.FirstOrDefault(x => predicate(x, item));
            if (existing is not null) {
                mergeAction(existing, item);
            }
            else {
                list.Add(item);
            }
        }
    }

    public static void ReplaceBy<T>(this List<T> list, IEnumerable<T> other, Func<T, T, bool> predicate) {
        foreach (var item in other) {
            var existing = list.FindIndex(x => predicate(x, item));
            if (existing != -1) {
                list[existing] = item;
            }
            else {
                list.Add(item);
            }
        }
    }

    public static bool ContainsAll<T>(this IEnumerable<T> list, IEnumerable<T> other) {
        return !other.Except(list).Any();
    }
}