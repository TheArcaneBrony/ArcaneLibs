using System.Diagnostics.CodeAnalysis;

namespace ArcaneLibs.Legacy;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class DownloadQueue {
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public int MaxConcurrentDownloads = 4;

    public readonly List<DownloadInfo> Queue = new();

    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public Func<DownloadQueue, bool> ShouldQueueMore = q => q.IsRunning.Count >= q.MaxConcurrentDownloads;

    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    // ReSharper disable once ConvertToConstant.Global
    public bool UpdatingQueue = true;
    public List<DownloadInfo> IsRunning => Queue.Where(x => x.Running).ToList();

    public bool AllDownloadsFinished => !Queue.Any(x => x.Progress < 1);
    private int MaxWidth => Queue.Max(x => x.FileName.Length);

    public void Add(DownloadInfo info) => Queue.Add(info);

    public async Task Run() {
        foreach (var n in Queue.Where(x => x.Size == 0)) File.Create(n.TargetFile).Close();

        Queue.RemoveAll(x => x.Size == 0);
        List<Task> tasks = new();
        while (!AllDownloadsFinished || UpdatingQueue) {
            while (!ShouldQueueMore(this))
                // Console.Clear();
                // Console.WriteLine(GetProgressBars());
                await Task.Delay(10);

            if (Queue.Any(x => x.Progress == 0 && !x.Running))
                tasks.Add(Queue.First(x => x.Progress == 0 && !x.Running).Download());
        }

        await Task.WhenAll(tasks);
    }

    public string GetProgressBars(int width = 100) {
        int p = (int)(Queue.Average(x => x.Progress) * width), np = width - p;
        var header = $"{"Total".PadRight(MaxWidth)} " +
                     $"[{new string('=', Math.Max(p, 0))}>" +
                     $"{new string(' ', Math.Max(np, 0))}]\n";
        return header + string.Join("\n",
            IsRunning.Select(x => $"{x.FileName.PadRight(MaxWidth)} " +
                                  $"[{new string('=', x.ProgressInt)}>" +
                                  $"{new string(' ', 100 - x.ProgressInt)}]"));
    }
}

[SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global", Justification = "Public API")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Public API")]
[SuppressMessage("ReSharper", "ConvertToConstant.Global", Justification = "Public API")]
public class DownloadInfo {
    public long BytesDownloaded;
    public string FileName = "";

    public bool Running;
    public long Size = 0;
    public string TargetFile = "";
    public string Url = "";
    public string TargetFilename => Url.Split("/").Last();
    public float Progress => (float)(BytesDownloaded / (double)Size);
    public int ProgressInt => (int)Math.Max(0, Math.Min(Progress * 100, 100));

    internal async Task Download() {
        Running = true;
        using var client = new HttpClient();
        var response = await client.GetStreamAsync(Url);
        var stream = File.OpenWrite(TargetFile);
        byte[] buffer = new byte[4096];
        int read;
        while ((read = await response.ReadAsync(buffer)) > 0) {
            BytesDownloaded += read;
            await stream.WriteAsync(buffer, 0, read);
        }

        BytesDownloaded = Size;
        Running = false;
        if (File.Exists(TargetFile)) File.Delete(TargetFile);
    }
}