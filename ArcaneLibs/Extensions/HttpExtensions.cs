using System.Reflection;
using System.Text;

namespace ArcaneLibs.Extensions;

public static class HttpExtensions {
    private static readonly string[] SensitiveHttpHeaders = [
        "Authorization"
    ];

    public static void ResetSendStatus(this HttpRequestMessage request) {
        typeof(HttpRequestMessage).GetField("_sendStatus", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(request, 0);
    }

    public static long GetContentLength(this HttpRequestMessage request) {
        if (request.Content == null) return 0;
        if (request.Content.Headers.ContentLength.HasValue) return request.Content.Headers.ContentLength.Value;
        return request.Content.ReadAsStream().Length;
        // return -1;
    }

    public static long GetContentLength(this HttpResponseMessage response) {
        // TODO: what does No Content mean for Content's value?
        // if (response.Content == null) return 0;
        if (response.Content.Headers.ContentLength.HasValue) return response.Content.Headers.ContentLength.Value;
        if (response.Content is StreamContent streamContent) {
            try {
                return streamContent.ReadAsStream().Length;
            }
            catch (NotSupportedException) {
                return -1;
            }
        }

        return -1;
    }

    public static string Summarise(this HttpRequestMessage request, bool includeQuery = true, bool includeHeaders = false, bool includeSensitiveHeaders = false,
        bool includeContentIfText = false, string[]? hideHeaders = null) {
        if (request.RequestUri == null) throw new NullReferenceException("RequestUri is null");
        var uri = request.RequestUri;

        var sb = new StringBuilder();

        // "HttpRequest<StreamContent>(123456): "
        sb.Append("HttpRequest");
        if (request.Content is not null) sb.Append($"<{request.Content.GetType()}>");
        sb.Append($"({request.GetHashCode()})");
        sb.Append(": ");

        // "GET https://example.com", includeQuery? ++ "?query=string"
        sb.Append($"{request.Method} {uri.FormatParts(includeQuery: includeQuery)}");

        // "(123 B)"
        if (request.Content != null) {
            var contentLength = request.GetContentLength();
            if (contentLength > 0) sb.Append($"({contentLength}");
        }

        // "Accept: text/html, text/plain"
        // "Content-Type: application/json"
        if (includeHeaders) {
            foreach (var header in request.Headers) {
                if (!includeSensitiveHeaders && SensitiveHttpHeaders.Contains(header.Key)) continue;
                if (hideHeaders != null && hideHeaders.Contains(header.Key)) continue;
                sb.Append($"\n{header.Key}: {string.Join(", ", header.Value)}");
            }
        }

        // "<data>"
        if (includeContentIfText && request.Content != null) {
            var content = request.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrEmpty(content)) sb.Append($"\n{content}");
        }

        return sb.ToString();
    }

    public static string Summarise(this HttpResponseMessage response, bool includeHeaders = false, bool includeSensitiveHeaders = false, bool includeContentIfText = false,
        string[]? hideHeaders = null) {
        if (response.RequestMessage == null) throw new NullReferenceException("RequestMessage is null");
        var request = response.RequestMessage;
        var uri = request.RequestUri ?? throw new NullReferenceException("RequestUri is null");

        var sb = new StringBuilder();

        // "HttpResponse<StreamContent>(123456): "
        sb.Append("HttpResponse");
        // TODO: what does No Content mean for Content's value?
        // if (response.Content is not null)
        sb.Append($"<{response.Content.GetType()}>");
        sb.Append($"({response.GetHashCode()})");
        sb.Append(": ");

        // "GET https://example.com", includeQuery? ++ "?query=string"
        sb.Append($"{request.Method} {uri.Scheme}://{uri.Host}{uri.AbsolutePath}");
        if (!string.IsNullOrEmpty(request.RequestUri.Query)) sb.Append(request.RequestUri.Query);

        // "(123 B)"
        // TODO: what does No Content mean for Content's value?
        // if (response.Content != null) {
            var contentLength = response.GetContentLength();
            if (contentLength > 0) sb.Append($" ({Util.BytesToString(contentLength)})");
            else sb.Append(" (stream)");
        // }

        // "200 OK"
        sb.Append($" {(int)response.StatusCode} {response.ReasonPhrase}");

        // "Content-Type: application/json"
        if (includeHeaders) {
            foreach (var header in response.Headers) {
                if (!includeSensitiveHeaders && SensitiveHttpHeaders.Contains(header.Key)) continue;
                if (hideHeaders != null && hideHeaders.Contains(header.Key)) continue;
                sb.Append($"\n{header.Key}: {string.Join(", ", header.Value)}");
            }
        }

        // "<data>"
        // TODO: what does No Content mean for Content's value?
        if (includeContentIfText) {
            var content = response.Content.ReadAsStringAsync().Result;
            if (!string.IsNullOrEmpty(content)) sb.Append($"\n{content}");
        }

        return sb.ToString();
    }
}