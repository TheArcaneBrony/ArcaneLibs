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

    public static void Replace<T>(this List<T> list, T oldItem, T newItem) {
        var index = list.IndexOf(oldItem);
        if (index != -1) {
            list[index] = newItem;
        }
        else {
            list.Add(newItem);
        }
    }

    public static bool ContainsAll<T>(this IEnumerable<T> list, IEnumerable<T> other) {
        return !other.Except(list).Any();
    }

    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, T value) {
        var currentGroup = new List<T>();
        foreach (var item in source) {
            if (item?.Equals(value) ?? false) {
                if (currentGroup.Count > 0) {
                    yield return currentGroup;
                    currentGroup = new List<T>();
                }
            }
            else currentGroup.Add(item);
        }

        if (currentGroup.Count > 0) {
            yield return currentGroup;
        }
    }

    public static IEnumerable<IEnumerable<T>> SplitBy<T>(this IEnumerable<T> source, Func<T, bool> predicate) {
        var currentGroup = new List<T>();
        foreach (var item in source) {
            if (predicate(item)) {
                if (currentGroup.Count > 0) {
                    yield return currentGroup;
                    currentGroup = new List<T>();
                }
            }
            else currentGroup.Add(item);
        }

        if (currentGroup.Count > 0) {
            yield return currentGroup;
        }
    }
}