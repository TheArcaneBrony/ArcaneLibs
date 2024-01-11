using System.Reflection;
using System.Text.Json.Serialization;

namespace ArcaneLibs.Extensions;

public static class JsonExtensions {
    public static string? GetJsonPropertyNameOrNull(this MemberInfo member) {
        var attrs = member.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false);
        return attrs.Length == 0 ? null : ((JsonPropertyNameAttribute)attrs[0]).Name;
    }

    public static string GetJsonPropertyName(this MemberInfo member) => member.GetJsonPropertyNameOrNull() ?? member.Name;
}