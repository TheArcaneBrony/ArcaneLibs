using System.Web;

namespace ArcaneLibs.Extensions;

public static class UriExtensions {
    public static Uri AddQuery(this Uri uri, string name, string value) {
        var location = uri.OriginalString.Split('?')[0];
        var query = uri.OriginalString.Split('?').Skip(1).FirstOrDefault();
        var newQuery = HttpUtility.ParseQueryString(query ?? "");
        newQuery[name] = value;
        // Console.WriteLine("OriginalString: " + uri.OriginalString);
        
        return new Uri(location + "?" + newQuery, uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
    }
}