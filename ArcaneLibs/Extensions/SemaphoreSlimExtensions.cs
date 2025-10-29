namespace ArcaneLibs.Extensions;

public static class SemaphoreSlimExtensions {
    public static async Task<T> RunWithLockAsync<T>(this SemaphoreSlim semaphore, Func<Task<T>> func) {
        await semaphore.WaitAsync();
        try {
            return await func();
        }
        finally {
            semaphore.Release();
        }
    }
}