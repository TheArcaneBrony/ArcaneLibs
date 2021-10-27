using System.Diagnostics;
using System.IO.Compression;
using System.Net;

namespace ArcaneLibs;

public class Util
{
    public static ulong ParseTime(string time)
    {
        string timestring = time[0..^1];
        if (ulong.TryParse(timestring, out ulong seconds))
        {
            return time.Last() switch
            {
                's' => seconds,
                'm' => seconds * 60,
                'h' => seconds * 60 * 60,
                'd' => seconds * 60 * 60 * 24,
                'w' => seconds * 60 * 60 * 24 * 7,
                'o' => seconds * 60 * 60 * 24 * 30,
                'y' => seconds * 60 * 60 * 24 * 365,
                _ => seconds
            };
        }

        return seconds;
    }
    public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static void DownloadFile(string contextName, string url, string filename, bool overwrite = false,
            bool extract = false, bool printProgress = true)
        {

            WebClient wc = new WebClient();
            if (File.Exists(filename) && !overwrite)
            {
                Console.WriteLine($"Not downloading {filename}, file exists.");
                return;
            }
            else if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            if(printProgress)
                wc.DownloadProgressChanged += (_, args) =>
                {
                    Console.Write($"Downloading {contextName}... {args.ProgressPercentage}%\r");
                };
            wc.DownloadFileCompleted += (_, _) =>
            {
                //Console.WriteLine(printProgress?"\n":"" + $"Finished downloading {contextName}");
            };
            wc.DownloadFileTaskAsync(new Uri(url), filename).Wait();
            Console.WriteLine($"Downloading {contextName}... [DONE]");
            if (extract)
            {
                Console.WriteLine($"Extracting {contextName}...");
                ZipFile.ExtractToDirectory(filename,contextName, overwrite);
                File.Delete(filename);
            }
            while(wc.IsBusy) Thread.Sleep(1000);
        }

        public static int Min(params int[] numbers)
        {
            return numbers.Min();
        }
        public static double Min(params double[] numbers)
        {
            return numbers.Min();
        }
        public static int Max(params int[] numbers)
        {
            return numbers.Max();
        }
        public static double Max(params double[] numbers)
        {
            return numbers.Max();
        }

        public static Dictionary<string, int> GetGitCommitCounts()
        {
            Dictionary<string, int> commitCounts = new();
            ProcessStartInfo psi = new ProcessStartInfo("git", "shortlog -s -n --all")
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true
            };
            Process proc = Process.Start(psi);
            while (!proc.StandardOutput.EndOfStream)
            {
                string line = proc.StandardOutput.ReadLine() ?? "";
                if (line.Length > 4)
                {
                    string[] commits = line.Trim().Split("\t");
                    if (commitCounts.ContainsKey(commits[1] ?? "Unknown"))
                    {
                        commitCounts[commits?[1] ?? "Unknown"] += int.Parse(commits?[0] ?? "-1");
                    }
                    else commitCounts.Add(commits?[1] ?? "Unknown", int.Parse(commits?[0]??"-1"));
                }
            }
            return commitCounts;
        }
        /// <summary>
        /// Writes file if different.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="text">Text to write</param>
        /// <returns>Wether the file was changed</returns>
        public static bool WriteAllTextIfDifferent(string path, string text)
        {
            string content = "";
            if (File.Exists(path))
            {
                content = File.ReadAllText(path);
            }
            if (content != text)
            {
                File.WriteAllText(path, text);
                return true;
            }

            return false;
        }
}