using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace ArcaneLibs.Legacy;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class CsvTools {
    public static string ToCsvHeader<T>(string separator) {
        var t = typeof(T);
        var fields = t.GetProperties();
        var header = string.Join(separator, fields.Select(f => f.Name).ToArray());
        return header;
    }

    public static string ToCsv<T>(string separator, IEnumerable<T> objectlist) where T : notnull {
        var t = typeof(T);
        var fields = t.GetProperties();

        var csvdata = new StringBuilder();

        foreach (var o in objectlist)
            csvdata.AppendLine(ToCsvFields(separator, fields, o));

        return csvdata.ToString();
    }

    public static string ToCsvFields(string separator, PropertyInfo[] fields, object o) {
        var line = new StringBuilder();

        foreach (var f in fields) {
            if (line.Length > 0)
                line.Append(separator);

            var x = f.GetValue(o);

            if (x != null)
                line.Append(x);
        }

        return line.ToString();
    }
}