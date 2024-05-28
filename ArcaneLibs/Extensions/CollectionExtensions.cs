namespace ArcaneLibs.Extensions;

public static class CollectionExtensions {
    public static T[] Add<T>(this T[] array, T item) {
        Array.Resize(ref array, array.Length + 1);
        array[^1] = item;
        return array;
    }

    //return task results async without preserving order
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<Task<T>> tasks, bool skipExceptions = false) {
        var taskList = tasks.ToList();
        while (taskList.Count > 0) {
            var task = await Task.WhenAny(taskList);
            taskList.Remove(task);
            if(skipExceptions && task.IsFaulted) continue; 
            yield return await task;
        }
    }
    
    public static int GetWidth<T>(this T[,] array) => array.GetLength(1);
    public static int GetHeight<T>(this T[,] array) => array.GetLength(0);
}