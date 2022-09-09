namespace ArcaneLibs.Extensions;

public static class BooleanExtensions
{
    public static string Toggle(this ref bool @bool)
    {
        return (@bool ^= true) ? "enabled" : "disabled";
    }

    public static (bool, string) ToggleAlt(this bool @bool)
    {
        return (@bool ^= true, @bool ? "enabled" : "disabled");
    }

    public static string ToEnglish(this bool @bool)
    {
        return @bool ? "enabled" : "disabled";
    }
}