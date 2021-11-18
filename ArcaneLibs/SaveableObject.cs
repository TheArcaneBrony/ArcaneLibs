using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace ArcaneLibs; 

public class SaveableObject<T> where T : new() {
    public static T Read(string filename = "") {
        if (filename == "") {
            Type callerType = typeof(T);
            filename = callerType.Name + ".json";
        }

        return (File.Exists(filename)
            ? JsonConvert.DeserializeObject<T>(File.ReadAllText(filename))
            : new T()) ?? new T();
    }

    public void Save(string filename = "")
    {
        if (filename == "") {
            Type callerType = typeof(T);
            filename = callerType.Name + ".json";
        }

        File.WriteAllText(filename, JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}