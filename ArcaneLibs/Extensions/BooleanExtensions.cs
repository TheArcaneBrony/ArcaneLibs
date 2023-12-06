namespace ArcaneLibs.Extensions;

public static class BooleanExtensions {
    public static string Toggle(this ref bool @bool) => (@bool ^= true) ? "enabled" : "disabled";

    public static (bool, string) ToggleAlt(this bool @bool) => (@bool ^= true, @bool ? "enabled" : "disabled");

    public static string ToEnglish(this bool @bool) => @bool ? "enabled" : "disabled";
}