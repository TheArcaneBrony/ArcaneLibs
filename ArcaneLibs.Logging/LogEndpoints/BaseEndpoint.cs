namespace ArcaneLibs.Logging.LogEndpoints;

public class BaseEndpoint {
    public virtual void Write(string text) =>
        Console.WriteLine("ArcaneLibs.Logging: invalid endpoint type: Write not implemented");
}