using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ArcaneLibs.Extensions;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API: extension methods")]
public static class StringExtensions {
    /// <summary>
    /// Test if a string contains another string, case insensitive
    /// </summary>
    /// <param name="haystack">String to search in</param>
    /// <param name="needle">String to find</param>
    /// <returns>Whether the string was found</returns>
    public static bool ContainsAnyCase(this string haystack, string needle) => haystack.Contains(needle, StringComparison.CurrentCultureIgnoreCase);

    public static bool ContainsAnyOf(this string str, IEnumerable<string> test) => test.Any(item => str.Contains(item, StringComparison.CurrentCultureIgnoreCase));

    public static bool MatchesAny(this string str, IEnumerable<string> regex, RegexOptions options) //check if string contains any lowercase instance of any item
    {
        foreach (var item in regex)
            if (Regex.Matches(str, item, options).Count > 0) {
                Console.WriteLine($"A message matched \"{item}\":\n{str}");
                return true;
            }

        return false;
    }

    //check if string starts with any lowercase instance of any item
    public static bool StartsWithAnyOf(this string str, IEnumerable<string> test) => test.Any(item => str.StartsWith(item, StringComparison.CurrentCultureIgnoreCase));

    public static bool StartsWithAnyOf(this string str, params string[] test) => StartsWithAnyOf(str, test.ToList());

    public static string ContentOrEmtpy(this string str) //check empty string if null
        => string.IsNullOrEmpty(str) ? string.Empty : str;

    public static int CountInstances(this string haystack, string needle) => haystack.Select((_, i) => haystack[i..]).Count(sub => sub.StartsWith(needle));

    public static int CountInstancesOfAll(this string haystack, IEnumerable<string> needles) {
        var instances = 0;
        foreach (var needle in needles) instances += haystack.CountInstances(needle);

        return instances;
    }

    public static void WriteAllTextFailSafe(this string text, string path, int maxAttempts = 10) {
        var success = false;
        while (maxAttempts-- > 0 && !success)
            try {
                File.WriteAllText(path, text);
                success = true;
            }
            catch {
                maxAttempts = -1;
                Console.WriteLine($"Failed to save file {path}, attempts left: {maxAttempts}");
            }
    }

    private static readonly char[] ArgumentSeparator = [' '];

    public static string[] ParseArguments(this string text) =>
        text.Split('"')
            .Select((element, index) => index % 2 == 0 // If even index
                ? element.Split(ArgumentSeparator, StringSplitOptions.RemoveEmptyEntries) // Split the item
                : new[] { element }) // Keep the entire item
            .SelectMany(element => element).ToArray();

    public static IEnumerable<byte> AsBytes(this string str) => Encoding.UTF8.GetBytes(str);

    public static string UrlEncode(this string str) => HttpUtility.UrlEncode(str);
    public static string UrlDecode(this string str) => HttpUtility.UrlDecode(str);
}