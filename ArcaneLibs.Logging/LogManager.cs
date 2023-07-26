using System.Diagnostics;
using System.Runtime.CompilerServices;
using ArcaneLibs.Logging.LogEndpoints;

namespace ArcaneLibs.Logging;

public class LogManager {
    private List<BaseEndpoint> _endpoints = new();
    public string Prefix = "";
    public string MessagePrefix = "";
    public bool LogTime = false;

    public void AddEndpoint(BaseEndpoint e) => _endpoints.Add(e);

    /// <summary>
    /// Sends text to all endpoints
    /// </summary>
    /// <param name="message">Text to send</param>
    /// <param name="file">Filename (auto)</param>
    /// <param name="line">Line number (auto)</param>
    public void Log(string message,
        [CallerFilePath] string file = null,
        [CallerLineNumber] int line = 0) {
        foreach (var endpoint in _endpoints)
            endpoint.Write((LogTime ? $"[{DateTime.Now:hh:mm:ss}]" : "") +
                           $"{Prefix}{Path.GetFileName(file)}:{line} {MessagePrefix}{message}");
    }

    /// <summary>
    /// Sends text to all endpoints in debug builds
    /// </summary>
    /// <param name="message">Text to send</param>
    /// <param name="file">Filename (auto)</param>
    /// <param name="line">Line number (auto)</param>
    [Conditional("DEBUG")]
    public void LogDebug(string message,
        [CallerFilePath] string file = null,
        [CallerLineNumber] int line = 0) =>
        Log(message, file, line);

    public void LogSplit(string separator, [CallerFilePath] string file = null,
        [CallerLineNumber] int line = 0, params object[] parts) =>
        Log(string.Join(separator, parts), file, line);
}