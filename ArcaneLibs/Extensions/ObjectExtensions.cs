using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ArcaneLibs.Extensions;

public static class ObjectExtensions
{
    public static void SaveToJsonFile(this object @object, string filename) // save object to json file
    {
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
        }
    }
}