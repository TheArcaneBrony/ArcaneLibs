using System.Reflection;
using System.Security.Cryptography;

namespace ArcaneLibs;

public static class FileUtils {
    public static async Task<string> GetFileSha384Async(string path) {
        using var sha384 = SHA384.Create();
        await using var stream = File.OpenRead(path);
        return Convert.ToBase64String(await sha384.ComputeHashAsync(stream));
    }
    
    public static string GetBinDir() {
        var assembly = Assembly.GetEntryAssembly();
        var uri = new Uri(assembly!.Location);
        return Path.GetDirectoryName(uri.LocalPath)!;
    }

    public static IEnumerable<FileSystemInfo> EnumerateDirectory(string path) {
        var contents = Directory.GetFileSystemEntries(path);
        foreach (var entry in contents) {
            if (File.Exists(entry)) {
                yield return new FileInfo(entry);
            } else if (Directory.Exists(entry)) {
                yield return new DirectoryInfo(entry);
            }
        }
    }
}