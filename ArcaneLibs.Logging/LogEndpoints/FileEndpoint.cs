namespace ArcaneLibs.Logging.LogEndpoints;

public class FileEndpoint : BaseEndpoint {
    private readonly StreamWriter _stream;

    public FileEndpoint(string path, bool append) {
        _stream = new StreamWriter(path, append);
        _stream.AutoFlush = true;
    }

    public override void Write(string text) => _stream.WriteLine(text);
}