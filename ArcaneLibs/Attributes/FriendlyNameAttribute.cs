using System.Reflection;

namespace ArcaneLibs.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
public class FriendlyNameAttribute : Attribute {
    public required string Name { get; set; }
    public string? NamePlural { get; set; }
}

public static class FriendlyNameAttributeExtensions {
    public static string? GetFriendlyNameOrNull(this MemberInfo type) {
        var attrs = type.GetCustomAttributes(typeof(FriendlyNameAttribute), false);
        return attrs.Length == 0 ? null : ((FriendlyNameAttribute)attrs[0]).Name;
    }

    public static string? GetFriendlyNamePluralOrNull(this MemberInfo type) {
        var attrs = type.GetCustomAttributes(typeof(FriendlyNameAttribute), false);
        return attrs.Length == 0 ? null : ((FriendlyNameAttribute)attrs[0]).NamePlural;
    }

    public static string GetFriendlyName(this MemberInfo type) => type.GetFriendlyNameOrNull() ?? type.Name;

    public static string GetFriendlyNamePlural(this MemberInfo type) => type.GetFriendlyNamePluralOrNull() ?? type.Name;
}