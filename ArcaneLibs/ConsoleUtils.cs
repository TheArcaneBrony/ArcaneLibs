namespace ArcaneLibs;

public static class ConsoleUtils {
    private static string PreviousConsoleColor { get; set; } = ColorSequence(255, 255, 255);

    public static string ColorSequence(byte r, byte g, byte b) {
        return $"\x1b[38;2;{r};{g};{b}m";
    }

    public static void SetConsoleColor(byte r, byte g, byte b) {
        Console.Write(ColorSequence(r, g, b));
    }

    public static string ColoredString(string text, byte r, byte g, byte b) {
        return $"{ColorSequence(r, g, b)}{text}{PreviousConsoleColor}";
    }
}