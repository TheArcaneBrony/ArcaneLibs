using System.Collections.Concurrent;
using System.Diagnostics;

namespace ArcaneLibs;

public class Timing {
    public bool PrintTimings;

    public Stopwatch StartTiming(string name, int timeoutMs = 60000) {
        var sw = Timings.GetOrAdd(name, (_) => {
            var lsw = new Stopwatch();
            lsw.Start();
            return lsw;
        });
        sw.Reset();
        sw.Start();
        if (PrintTimings) Console.WriteLine($"Started timer {name}.");

        new Thread(() => {
            if (Timings.TryGetValue(name, out var timing))
                while (timing.IsRunning) {
                    if (timing.ElapsedMilliseconds >= timeoutMs)
                        Fail(name);
                    Thread.Sleep(250);
                }
            else
                Console.WriteLine("Could not find timing with name " + name);
        }).Start();
        return sw;
    }

    public Stopwatch StopTiming(string name) {
        if (!Timings.TryGetValue(name, out var sw)) throw new Exception("Timing does not exist!");

        sw.Stop();
        if (PrintTimings) Console.WriteLine($"Stopped timer {name}: {sw.ElapsedMilliseconds} ms.");

        return sw;
    }

    public List<Stopwatch> StopTimingAll(string name) {
        List<Stopwatch> sws = new();
        foreach (var sw in Timings.Where(x => x.Key.StartsWith(name)))
            sws.Add(StopTiming(sw.Key));
        Console.WriteLine($"Stopped {sws.Count} timings in {name}.");
        return sws;
    }

    public Stopwatch GotoNext(Stopwatch old, string name, bool muted = false) {
        old.Stop();
        if (!muted && PrintTimings)
            Console.WriteLine(
                $"Stopped timer {Timings.FirstOrDefault(x => x.Value == old).Key}: {old.ElapsedMilliseconds} ms, moving on to {name}.");

        var sw = Timings.GetOrAdd(name, (_) => {
            var lsw = new Stopwatch();
            lsw.Start();
            return lsw;
        });
        sw.Reset();
        sw.Start();
        return sw;
    }

    public Stopwatch StopTiming(Stopwatch old) {
        old.Stop();
        if (PrintTimings)
            Console.WriteLine(
                $"Stopped timer {Timings.FirstOrDefault(x => x.Value == old).Key}: {old.ElapsedMilliseconds} ms.");

        return old;
    }

    public Stopwatch Fail(string name) {
        if (!Timings.TryGetValue(name, out var sw)) throw new Exception("Timing does not exist!");

        sw.Stop();
        return sw;
    }

    public ConcurrentDictionary<string, Stopwatch> Timings = new();
}
