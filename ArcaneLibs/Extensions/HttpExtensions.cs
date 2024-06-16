using System.Reflection;

namespace ArcaneLibs.Extensions;

public static class HttpExtensions {
    public static void ResetSendStatus(this HttpRequestMessage request) {
        typeof(HttpRequestMessage).GetField("_sendStatus", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(request, 0);
    }
    
}