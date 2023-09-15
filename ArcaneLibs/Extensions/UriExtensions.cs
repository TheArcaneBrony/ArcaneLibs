using System.Web;

namespace ArcaneLibs.Extensions;

public static class UriExtensions {

    public static Uri AddQuery(this Uri uri, string name, string value) {
        var ub = new UriBuilder(uri);
        var qs = HttpUtility.ParseQueryString(uri.Query);
        qs[name] = value;
        ub.Query = qs.ToString();
        return ub.Uri;
    }

}
