using System.Diagnostics.CodeAnalysis;

namespace ArcaneLibs;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
public class MathUtil {
    public static int Map(int value, int originalStart, int originalEnd, int newStart, int newEnd) =>
        (int)Map((long)value, originalStart, originalEnd, newStart, newEnd);

    public static long Map(long value, int originalStart, int originalEnd, int newStart, int newEnd) {
        var scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
        return (int)(newStart + ((value - originalStart) * scale));
    }
}