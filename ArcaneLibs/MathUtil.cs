using System.Diagnostics.CodeAnalysis;

namespace ArcaneLibs;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
public static class MathUtil {
    public static int Map(int value, int originalStart, int originalEnd, int newStart, int newEnd) =>
        (int)Map((long)value, originalStart, originalEnd, newStart, newEnd);

    public static long Map(long value, long originalStart, long originalEnd, long newStart, long newEnd) {
        var scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
        return (long)(newStart + ((value - originalStart) * scale));
    }

    public static float Map(float value, float originalStart, float originalEnd, float newStart, float newEnd) {
        var scale = (newEnd - newStart) / (originalEnd - originalStart);
        var res = newStart + ((value - originalStart) * scale);
        // Console.WriteLine($"MapFloat({value}, {originalStart}, {originalEnd}, {newStart}, {newEnd}) = {res} @ {scale}");
        return res;
    }
    public static double Map(double value, double originalStart, double originalEnd, double newStart, double newEnd) {
        var scale = (newEnd - newStart) / (originalEnd - originalStart);
        var res = newStart + ((value - originalStart) * scale);
        // Console.WriteLine($"MapDouble({value}, {originalStart}, {originalEnd}, {newStart}, {newEnd}) = {res} @ {scale}");
        return res;
    }
}