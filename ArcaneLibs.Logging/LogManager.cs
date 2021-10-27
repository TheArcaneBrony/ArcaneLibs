using System.Diagnostics;
using System.Runtime.CompilerServices;
using ArcaneLibs.Logging.LogEndpoints;

namespace ArcaneLibs.Logging;

public class LogManager
{
    private List<BaseEndpoint> _endpoints = new List<BaseEndpoint>();

    public void AddEndpoint(BaseEndpoint e)
    {
        _endpoints.Add(e);
    }
    /// <summary>
    /// Sends text to all endpoints
    /// </summary>
    /// <param name="message">Text to send</param>
    /// <param name="file">Filename (auto)</param>
    /// <param name="line">Line number (auto)</param>
    public void Log(string message,
        [CallerFilePath] string file = null,
        [CallerLineNumber] int line = 0)          
    {
        foreach (var endpoint in _endpoints)
        {
            endpoint.Write($"{Path.GetFileName(file)}:{line} {message}");
        }
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
        [CallerLineNumber] int line = 0)          
    {
        foreach (var endpoint in _endpoints)
        {
            endpoint.Write($"{Path.GetFileName(file)}:{line} {message}");
        }
    }
    public void LogSplit(string separator, [CallerFilePath] string file = null,
        [CallerLineNumber] int line = 0, params object[] parts)
    {
        Log(string.Join(separator, parts));
    }
}