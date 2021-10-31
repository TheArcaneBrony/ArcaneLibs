using System.Reflection;
using System.Text;

namespace ArcaneLibs;

public class CsvTools
{
    public static string ToCsvHeader<T>(string separator)
    {
        Type t = typeof(T);
        PropertyInfo[] fields = t.GetProperties();
        string header = string.Join(separator, fields.Select(f => f.Name).ToArray());
        return header;
    }

    public static string ToCsv<T>(string separator, IEnumerable<T> objectlist)
    {
        Type t = typeof(T);
        PropertyInfo[] fields = t.GetProperties();
            
        StringBuilder csvdata = new StringBuilder();
            
        foreach (var o in objectlist) 
            csvdata.AppendLine(ToCsvFields(separator, fields, o));

        return csvdata.ToString();
    }

    public static string ToCsvFields(string separator, PropertyInfo[] fields, object o)
    {
        StringBuilder linie = new StringBuilder();

        foreach (var f in fields)
        {
            if (linie.Length > 0)
                linie.Append(separator);

            var x = f.GetValue(o);

            if (x != null)
                linie.Append(x.ToString());
        }

        return linie.ToString();
    }
}