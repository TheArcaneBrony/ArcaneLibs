namespace ArcaneLibs;

public class MathUtil
{
    public static int Map(int value, int originalStart, int originalEnd, int newStart, int newEnd)
    {
        return (int) Map((long) value, originalStart, originalEnd, newStart, newEnd);
    }

    public static long Map(long value, int originalStart, int originalEnd, int newStart, int newEnd)
    {
        double scale = (double)(newEnd - newStart) / (originalEnd - originalStart);
        return (int)(newStart + ((value - originalStart) * scale));
    }
}