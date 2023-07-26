using System.Text.Json;
using ArcaneLibs.Extensions;

namespace ArcaneLibs;

public class SaveableObject<T> where T : new() {
    public static T Read(string filename = "") {
        if (filename == "") {
            var callerType = typeof(T);
            filename = callerType.Name + ".json";
        }

        return (File.Exists(filename)
            ? JsonSerializer.Deserialize<T>(File.ReadAllText(filename))
            : new T()) ?? new T();
    }

    public void Save(string filename = "") {
        if (filename == "") {
            var callerType = typeof(T);
            filename = callerType.Name + ".json";
        }

        File.WriteAllText(filename, this.ToJson());
    }
}
