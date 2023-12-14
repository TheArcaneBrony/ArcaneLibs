namespace ArcaneLibs.Extensions;

public static class BooleanExtensions {
#pragma warning disable CS0665 // Assignment in conditional expression is always constant
    public static string Toggle(this ref bool @bool) => (@bool ^= true) ? "enabled" : "disabled";
#pragma warning restore CS0665 // Assignment in conditional expression is always constant

    public static (bool, string) ToggleAlt(this bool @bool) => (@bool ^= true, @bool ? "enabled" : "disabled");

    public static string ToEnglish(this bool @bool) => @bool ? "enabled" : "disabled";
}