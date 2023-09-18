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
            Util.WriteAllTextIfDifferent(filename, ToJson(@object, true, false, false));

    public static string ToJson(this object obj, bool indent = true, bool ignoreNull = false,
        bool unsafeContent = false) {
        var jso = new JsonSerializerOptions();
        if (indent) jso.WriteIndented = true;
        if (ignoreNull) jso.IgnoreNullValues = true;
        if (unsafeContent) jso.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        return JsonSerializer.Serialize(obj, obj.GetType(), jso);
    }
}
