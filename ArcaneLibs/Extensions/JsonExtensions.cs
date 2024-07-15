using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ArcaneLibs.Extensions;

public static class JsonExtensions {
    public static string? GetJsonPropertyNameOrNull(this MemberInfo member) {
        var attrs = member.GetCustomAttributes(typeof(JsonPropertyNameAttribute), false);
        return attrs.Length == 0 ? null : ((JsonPropertyNameAttribute)attrs[0]).Name;
    }

    public static string GetJsonPropertyName(this MemberInfo member) => member.GetJsonPropertyNameOrNull() ?? member.Name;

    public static JsonNode? SortProperties(this JsonNode? node, bool recursive = true) => node.SortProperties(StringComparer.Ordinal, recursive);

    public static JsonNode? SortProperties(this JsonNode? node, IComparer<string> comparer, bool recursive = true) {
        if (node is JsonObject obj) {
            var properties = obj.ToList();
            obj.Clear();
            foreach (var pair in properties.OrderBy(p => p.Key, comparer))
                obj.Add(new(pair.Key, recursive ? pair.Value.SortProperties(comparer, recursive) : pair.Value));
        }
        else if (node is JsonArray array) {
            foreach (var n in array)
                n.SortProperties(comparer, recursive);
        }

        return node;
    }
    
    public static JsonNode? CanonicalizeNumbers(this JsonNode? node, bool recursive = true) {
        if (node is JsonValue val) {
            if (val.GetValueKind() == JsonValueKind.Number) {
                if (val.TryGetValue(out double d)) {
                    val.Parent![val.GetPropertyName()] = d switch {
                        double.NaN => throw new JsonException("Cannot canonicalize NaN."),
                        double.NegativeInfinity => throw new JsonException("Cannot canonicalize -Infinity."),
                        double.PositiveInfinity => throw new JsonException("Cannot canonicalize Infinity."),
                        double.NegativeZero => 0,
                        _ => Math.Abs(d % 1) < double.Epsilon ? (long)d : throw new JsonException("Cannot canonicalize non-integral numbers.")
                    };
                }
            }
        }
        else if (node is JsonObject obj) {
            // foreach (var pair in obj)
                // pair.Value.CanonicalizeNumbers(recursive);
            foreach (var key in obj.ToList().Select(x=>x.Key)) {
                obj[key].CanonicalizeNumbers(recursive);
            }
        }
        else if (node is JsonArray array) {
            foreach (var n in array)
                n.CanonicalizeNumbers(recursive);
        }

        return node;
    }
}