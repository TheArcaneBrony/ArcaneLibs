using System.Text;
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

    public static Uri EnsureAbsolute(this Uri uri, Uri baseUri) {
        if (uri.IsAbsoluteUri) {
            if (baseUri is not null && !uri.OriginalString.StartsWith(baseUri.OriginalString)) {
                throw new ArgumentException("Uri is not a child of the baseUri");
            }

            return uri;
        }

        return new Uri(baseUri, uri);
    }

    public static string FormatParts(this Uri uri, bool includeQuery = true, bool includeFragment = true) {
        var sb = new StringBuilder();
        sb.Append(uri.Scheme).Append("://").Append(uri.Host).Append(uri.AbsolutePath);
        if (includeQuery && !string.IsNullOrEmpty(uri.Query)) sb.Append(uri.Query);
        if (includeFragment && !string.IsNullOrEmpty(uri.Fragment)) sb.Append(uri.Fragment);
        return sb.ToString();
    }
}