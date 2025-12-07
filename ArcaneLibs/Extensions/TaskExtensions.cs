namespace ArcaneLibs.Extensions;

public static class TaskExtensions {
    public static async Task<T?> OrNull<T>(this Task<T> task) where T : class {
        try {
            return await task;
        }
        catch {
            return null;
        }
    }

    public static async Task<T> RetryOnException<T>(this Task<T> task, int retries = 0, int delayMs = 100) {
        while (true) {
            try {
                return await task;
            }
            catch {
                if (retries-- <= 0) throw;
                await Task.Delay(delayMs);
            }
        }
    }
}