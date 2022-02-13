using System.Net;

namespace ArcaneLibs;

public class DownloadQueue {
    public List<DownloadInfo> queue = new();
    List<DownloadInfo> running => queue.Where(x => x.Running).ToList();

    public void Add(DownloadInfo info) => queue.Add(info);

    public int MaxConcurrentDownloads = 4;
    public bool UpdatingQueue = true;

    public bool AllDownloadsFinished => !queue.Any(x => x.Progress < 1);
    private int maxWidth => queue.Max(x=>x.FileName.Length);

    public async Task Run() {
        foreach (var n in queue.Where(x=>x.Size == 0)) {
            File.Create(n.TargetFile).Close();
        }

        queue.RemoveAll(x => x.Size == 0);
        while (!AllDownloadsFinished || UpdatingQueue) {
            while (running.Count >= MaxConcurrentDownloads) {
                // Console.Clear();
                // Console.WriteLine(GetProgressBars());
                await Task.Delay(10);
            }

            if (queue.Any(x => x.Progress == 0 && !x.Running)) queue.First(x => x.Progress == 0 && !x.Running).StartDownload();
        }
    }

    public string GetProgressBars(int width = 100) {
        int p = (int)(queue.Average(x => x.Progress)*width), np = width - p;
        string header = $"{"Total".PadRight(maxWidth)} " +
                        $"[{new string('=', Math.Max(p, 0))}>" +
                        $"{new string(' ', Math.Max(np, 0))}]\n";
        return header + string.Join("\n",
            running.Select(x => $"{x.FileName.PadRight(maxWidth)} " +
                                $"[{new string('=', x.ProgressInt)}>" +
                                $"{new string(' ', 100 - x.ProgressInt)}]"));
    }
}

public class DownloadInfo {
    public string TargetFile = "";
    public string FileName = "";
    public string Url = "";
    public long Size = 0;

    internal bool Running = false;
    public string TargetFilename => Url.Split("/").Last();
    internal long BytesDownloaded = 0;
    internal float Progress => (float)(BytesDownloaded / (double)Size);
    internal int ProgressInt => (int)Math.Max(0, Math.Min(Progress * 100, 100));

    internal void StartDownload() {
        Running = true;
        WebClient client = new WebClient();
        client.DownloadProgressChanged += (_, args) => BytesDownloaded = args.BytesReceived;
        client.DownloadFileCompleted += (_, _) => {
            BytesDownloaded = Size;
            Running = false;
        };
        if (File.Exists(TargetFile)) File.Delete(TargetFile);
        client.DownloadFileAsync(new Uri(Url), TargetFile);
    }
}