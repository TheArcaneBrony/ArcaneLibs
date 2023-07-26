using System.Diagnostics;

namespace ArcaneLibs.Extensions;

public static class TimeRelatedExtensions {
    public static string TimeSinceFormatted(this DateTimeOffset dto) {
        var ts = DateTime.Now - dto;
        return ts.TimeSinceFormatted();
    }

    public static string TimeSinceFormatted(this TimeSpan timeSpan) {
        var formatted = "";
        if (timeSpan.Days > 0) formatted += $"{timeSpan.Days}.";
        if (timeSpan.TotalHours > 0) formatted += $"{timeSpan.Hours:00}:";
        if (timeSpan.TotalMinutes > 0) formatted += $"{timeSpan.Minutes:00}{(timeSpan.Seconds % 2 == 1 ? "." : ":")}";
        if (timeSpan.TotalSeconds > 0) formatted += $"{timeSpan.Seconds:00}";
        return formatted;
    }

    public static double StopTiming(this Stopwatch sw) {
        sw.Stop();
        return Math.Round(sw.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond, 2);
    }
}