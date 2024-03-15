using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Net;

namespace ArcaneLibs;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public static class Util {
    public static ulong ParseTime(string time) {
        var timeString = time[..^1];
        if (ulong.TryParse(timeString, out var seconds))
            return time.Last() switch {
                's' => seconds,
                'm' => seconds * 60,
                'h' => seconds * 60 * 60,
                'd' => seconds * 60 * 60 * 24,
                'w' => seconds * 60 * 60 * 24 * 7,
                'o' => seconds * 60 * 60 * 24 * 30,
                'y' => seconds * 60 * 60 * 24 * 365,
                _ => seconds
            };

        return seconds;
    }

    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
        // Get the subdirectories for the specified directory.
        var dir = new DirectoryInfo(sourceDirName);

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}");

        var dirs = dir.GetDirectories();

        // If the destination directory doesn't exist, create it.
        Directory.CreateDirectory(destDirName);

        // Get the files in the directory and copy them to the new location.
        var files = dir.GetFiles();
        foreach (var file in files) {
            var temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, false);
        }

        // If copying subdirectories, copy them and their contents to new location.
        if (copySubDirs)
            foreach (var subdir in dirs) {
                var temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
    }

    // public static void DownloadFile(string contextName, string url, string filename, bool overwrite = false, bool extract = false, bool printProgress = true) {
    //     using var wc = new WebClient();
    //     if (File.Exists(filename) && !overwrite) {
    //         Console.WriteLine($"Not downloading {filename}, file exists.");
    //         return;
    //     }
    //
    //     if (File.Exists(filename)) File.Delete(filename);
    //
    //     if (printProgress)
    //         wc.DownloadProgressChanged += (_, args) => { Console.Write($"Downloading {contextName}... {args.ProgressPercentage}%\r"); };
    //     wc.DownloadFileCompleted += (_, _) => {
    //         //Console.WriteLine(printProgress?"\n":"" + $"Finished downloading {contextName}");
    //     };
    //     wc.DownloadFileTaskAsync(new Uri(url), filename).Wait();
    //     Console.WriteLine($"Downloading {contextName}... [DONE]");
    //     if (extract) {
    //         Console.WriteLine($"Extracting {contextName}...");
    //         ZipFile.ExtractToDirectory(filename, contextName, overwrite);
    //         File.Delete(filename);
    //     }
    //
    //     while (wc.IsBusy) Thread.Sleep(1000);
    // }

    public static int Min(params int[] numbers) => numbers.Min();

    public static double Min(params double[] numbers) => numbers.Min();

    public static int Max(params int[] numbers) => numbers.Max();

    public static double Max(params double[] numbers) => numbers.Max();

    public static long GetDirSizeRecursive(string dir) {
        if (!Directory.Exists(dir)) return 0;
        return Directory.GetDirectories(dir).Sum(GetDirSizeRecursive) +
               Directory.GetFiles(dir).Sum(x => new FileInfo(x).Length);
    }

    public static void RunCommandSync(string command, string args = "", bool silent = false) =>
        RunCommandInDirSync(Environment.CurrentDirectory, command, args, silent);

    public static void RunCommandInDirSync(string path, string command, string args = "", bool silent = false) =>
        Process.Start(new ProcessStartInfo {
            WorkingDirectory = path,
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = !silent
        })?.WaitForExit();

    public static string GetCommandOutputSync(string command, string args = "", bool silent = true, bool stdout = true, bool stderr = true) =>
        GetCommandOutputInDirSync(Environment.CurrentDirectory, command, args, silent, stdout, stderr);

    public static string GetCommandOutputInDirSync(string path, string command, string args = "", bool silent = true, bool stdout = true, bool stderr = true) {
        var psi = new ProcessStartInfo(command, args) {
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = path
        };
        var proc = Process.Start(psi);
        if (proc is null) throw new NullReferenceException("Process was null!");
        var output = "";
        while (!proc.StandardOutput.EndOfStream || !proc.StandardError.EndOfStream) {
            if (!proc.StandardOutput.EndOfStream) {
                var line = proc.StandardOutput.ReadLine() ?? "";
                if (stdout) output += line + "\n";
                if (!silent) Console.WriteLine(output);
            }

            if (!proc.StandardError.EndOfStream) {
                var line = proc.StandardError.ReadLine() ?? "";
                if (stderr) output += line + "\n";
                if (!silent) Console.WriteLine(output);
            }
        }

        return output.TrimEnd('\n');
    }

    public static IAsyncEnumerable<string> GetCommandOutputAsync(string command, string args = "", bool silent = true, bool stdout = true, bool stderr = true) =>
        GetCommandOutputInDirAsync(Environment.CurrentDirectory, command, args, silent, stdout, stderr);

    public static async IAsyncEnumerable<string> GetCommandOutputInDirAsync(string path, string command, string args = "", bool silent = true,
        bool stdout = true, bool stderr = true) {
        var psi = new ProcessStartInfo(command, args) {
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = path
        };
        var proc = Process.Start(psi);
        var output = "";
        while (!proc!.StandardOutput.EndOfStream || !proc.StandardError.EndOfStream || !proc.HasExited) {
            if (!proc.StandardOutput.EndOfStream) {
                var line = await proc.StandardOutput.ReadLineAsync() ?? "";
                if (stdout) yield return line;
                if (!silent) Console.WriteLine(output);
            }

            if (!proc.StandardError.EndOfStream) {
                var line = await proc.StandardError.ReadLineAsync() ?? "";
                if (stderr) yield return line;
                if (!silent) Console.WriteLine(output);
            }
        }
    }

    public static string BytesToString(long byteCount, int maxnums = 2) {
        string[] suf = ["B", "kiB", "MiB", "GiB", "TiB", "PiB", "EiB"]; //Longs run out around EB
        if (byteCount == 0)
            return "0 " + suf[0];
        var bytes = Math.Abs(byteCount);
        var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        var num = Math.Round(bytes / Math.Pow(1024, place), maxnums);
        return (Math.Sign(byteCount) * num) + " " + suf[place];
    }

    public static string SI_BytesToString(long byteCount, int maxnums = 2) {
        string[] suf = ["B", "kB", "MB", "GB", "TB", "PB", "EB"]; //Longs run out around EB
        if (byteCount == 0)
            return "0 " + suf[0];
        var bytes = Math.Abs(byteCount);
        var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1000)));
        var num = Math.Round(bytes / Math.Pow(1000, place), maxnums);
        return (Math.Sign(byteCount) * num) + " " + suf[place];
    }
}