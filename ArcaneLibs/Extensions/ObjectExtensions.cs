using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ArcaneLibs.Extensions;

public static class ObjectExtensions {
    public static void SaveToJsonFile(this object @object, string filename) // save object to json file
        =>
            /*
        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            DefaultValueHandling = DefaultValueHandling.Populate,
        };
        settings.Converters.Add(new StringEnumConverter());
        // serialise object
        string json = JsonConvert.SerializeObject(@object, Formatting.Indented, settings);
        // save to files
        try
        {
            Util.WriteAllTextIfDifferent(filename, json);
        }
        catch
        {
            // ignored
        }*/
            File.WriteAllText(filename, ToJson(@object, true, false, false));

    public static string ToJson(this object obj, bool indent = true, bool ignoreNull = false,
        bool unsafeContent = false) {
        var jso = new JsonSerializerOptions();
        if (indent) jso.WriteIndented = true;
        if (ignoreNull) jso.IgnoreNullValues = true;
        if (unsafeContent) jso.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        return JsonSerializer.Serialize(obj, obj.GetType(), jso);
    }

    public static T DeepClone<T>(this T obj) where T : class {
        var type = obj.GetType();
        if (type.IsValueType || type == typeof(string)) return obj;
        if (type.IsArray) {
            var elementType = Type.GetType(
                type.FullName.Replace("[]", string.Empty));
            var array = obj as Array;
            var copied = Array.CreateInstance(elementType, array.Length);
            for (var i = 0; i < array.Length; i++) {
                copied.SetValue(DeepClone(array.GetValue(i)), i);
            }

            return Convert.ChangeType(copied, obj.GetType()) as T;
        }

        if (type.IsClass) {
            var instance = Activator.CreateInstance(obj.GetType());
            var fields = type.GetFields(BindingFlags.Public |
                                        BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields) {
                var fieldValue = field.GetValue(obj);
                if (fieldValue == null) continue;
                field.SetValue(instance, DeepClone(fieldValue));
            }

            return (T)instance;
        }

        throw new ArgumentException("Unknown type");
    }

    public static (T left, T right) FindDifferencesDeep<T>(this T left, T right) where T : class {
        var type = left.GetType();
        if (type.IsValueType || type == typeof(string)) return (left, right);
        if (type.IsArray) {
            var elementType = Type.GetType(
                type.FullName.Replace("[]", string.Empty));
            var arrayLeft = left as Array;
            var arrayRight = right as Array;
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

        if (type.IsClass) {
            var instanceLeft = Activator.CreateInstance(left.GetType());
            var instanceRight = Activator.CreateInstance(right.GetType());
            var fields = type.GetFields(BindingFlags.Public |
                                        BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields) {
                var fieldValueLeft = field.GetValue(left);
                var fieldValueRight = field.GetValue(right);
                if (fieldValueLeft == null && fieldValueRight == null) continue;
                (fieldValueLeft, fieldValueRight) =
                    FindDifferencesDeep(fieldValueLeft, fieldValueRight);
                field.SetValue(instanceLeft, fieldValueLeft);
                field.SetValue(instanceRight, fieldValueRight);
            }

            return ((T)instanceLeft, (T)instanceRight);
        }

        throw new ArgumentException("Unknown type");
    }
}
