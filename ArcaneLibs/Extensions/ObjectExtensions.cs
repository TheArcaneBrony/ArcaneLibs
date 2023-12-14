using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArcaneLibs.Extensions;

[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API: extension functions")]
public static class ObjectExtensions {
    public static void SaveToJsonFile(this object? @object, string filename) => File.WriteAllText(filename, ToJson(@object));

    private static readonly Dictionary<byte, JsonSerializerOptions> OptionsCache = new();

    public static string ToJson(this object? obj, bool indent = true, bool ignoreNull = false, bool unsafeContent = false) {
        if (obj is null) return "";
        var cacheKey = (byte)((indent ? 1 : 0) | (ignoreNull ? 2 : 0) | (unsafeContent ? 4 : 0));
        var options = OptionsCache.GetOrCreate(cacheKey , _ => new JsonSerializerOptions {
            WriteIndented = indent,
            DefaultIgnoreCondition = ignoreNull ? JsonIgnoreCondition.WhenWritingNull : JsonIgnoreCondition.Never,
            Encoder = unsafeContent ? JavaScriptEncoder.UnsafeRelaxedJsonEscaping : null
        });
        return JsonSerializer.Serialize(obj, obj.GetType(), options);
    }

    public static T? DeepClone<T>(this T? obj) where T : class {
        Console.WriteLine($"DeepClone<{typeof(T)}>");
        if (obj is null) return null;
        var type = typeof(T);
        if (type.IsValueType || type == typeof(string)) return obj;
        if (type.IsArray) {
            if (type.FullName is null) throw new NullReferenceException($"{nameof(type.FullName)} is null!");
            var elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
            if (elementType is null) throw new NullReferenceException("Could not resolve array type!");
            var array = obj as Array;
            var copied = Array.CreateInstance(elementType, array!.Length);
            for (var i = 0; i < array.Length; i++) copied.SetValue(DeepClone(array.GetValue(i)), i);

            return Convert.ChangeType(copied, obj.GetType()) as T;
        }

        if (type.IsClass) {
            // ReSharper disable once SuggestVarOrType_SimpleTypes
            var instance = Activator.CreateInstance<T>();
            var fields = type.GetFields(BindingFlags.Public |
                                        BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields) {
                var fieldValue = field.GetValue(obj);
                if (fieldValue == null) continue;
                var newValue = DeepClone(fieldValue);
                Console.WriteLine($"{field.Name}: {fieldValue.GetType().Name} -> {newValue.GetType().Name}");
                field.SetValue(instance, newValue);
            }

            return instance;
        }

        throw new ArgumentException("Unknown type");
    }

    public static (T? left, T? right) FindDifferencesDeep<T>(this T? left, T? right) where T : class {
        // don't bother if either left or right are null
        if (left is null && right is not null) return (null, right);
        if (left is not null && right is null) return (left, null);
        if (left is null && right is null) return (null, null);

        var type = typeof(T);
        if (type.IsValueType || type == typeof(string)) return (left, right);
        if (type.IsArray) {
            if (type.FullName is null) throw new NullReferenceException($"{nameof(type.FullName)} is null!");
            var elementType = Type.GetType(type.FullName.Replace("[]", string.Empty));
            if (elementType is null) throw new NullReferenceException("Could not resolve array type!");
            if (left is Array arrayLeft && right is Array arrayRight) {
                var copiedLeft = Array.CreateInstance(elementType, arrayLeft.Length);
                var copiedRight = Array.CreateInstance(elementType, arrayRight.Length);
                for (var i = 0; i < arrayLeft.Length; i++) {
                    var tuple = FindDifferencesDeep(arrayLeft.GetValue(i), arrayRight.GetValue(i));
                    copiedLeft.SetValue(tuple.left, i);
                    copiedRight.SetValue(tuple.right, i);
                }

                return ((T)Convert.ChangeType(copiedLeft, left.GetType()),
                    (T)Convert.ChangeType(copiedRight, right.GetType()));
            }

            throw new InvalidCastException($"T is array, but either left or right are not!\nPassed values:\nLeft: {left!.GetType()}\nRight: {right!.GetType()}");
        }

        if (type.IsClass) {
            var instanceLeft = Activator.CreateInstance<T>();
            var instanceRight = Activator.CreateInstance<T>();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields) {
                var fieldValueLeft = field.GetValue(left);
                var fieldValueRight = field.GetValue(right);
                if (fieldValueLeft == null && fieldValueRight == null) continue;
                (fieldValueLeft, fieldValueRight) =
                    FindDifferencesDeep(fieldValueLeft, fieldValueRight);
                field.SetValue(instanceLeft, fieldValueLeft);
                field.SetValue(instanceRight, fieldValueRight);
            }

            return (instanceLeft, instanceRight);
        }

        throw new ArgumentException($"Unknown type: {type}");
    }
}