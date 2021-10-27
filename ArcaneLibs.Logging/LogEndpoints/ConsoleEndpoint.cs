using System.Xml;

namespace ArcaneLibs.Logging.LogEndpoints;

public class ConsoleEndpoint : BaseEndpoint
{
    public override void Write(string text)
    {
        Console.WriteLine(text);
    }
}