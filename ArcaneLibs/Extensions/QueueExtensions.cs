namespace ArcaneLibs.Extensions;

public static class QueueExtensions {
    public static IEnumerable<T> Dequeue<T>(this Queue<T> queue, int count) {
        for (var i = 0; i < count; i++) yield return queue.Dequeue();
    }

    public static IEnumerable<T> Peek<T>(this Queue<T> queue, int count) {
        for (var i = 0; i < count; i++) yield return queue.ElementAt(i);
    }

    public static void Drop<T>(this Queue<T> queue, int count) {
        for (var i = 0; i < count; i++) queue.Dequeue();
    }
}