using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Unidecode.NET;

namespace ArcaneLibs.Extensions;

public static class StringExtensions
{
    public static bool
        ContainsAnyCase(this string str,
            string test) //check if lowercase string contains variable "test" (should be lowercase, otherwise always returns false)
    {
        return str.ToLower().Contains(test);
    }

    public static bool
        ContainsAnyOf(this string str,
            IEnumerable<string> test) //check if string contains any lowercase instance of any item
    {
        foreach (string item in test)
        {
            if (str.ToLower().Contains(item.ToLower()))
            {
                return true;
            }
        }

        return false;
    }

    public static bool
        MatchesAny(this string str, IEnumerable<string> regex,
            RegexOptions options) //check if string contains any lowercase instance of any item
    {
        foreach (string item in regex)
        {
            if (Regex.Matches(str, item, options).Count > 0)
            {
                Console.WriteLine($"A message matched \"{item}\":\n{str}");
                return true;
            }
        }

        return false;
    }

    public static bool
        StartsWithAnyOf(this string str,
            IEnumerable<string> test) //check if string starts with any lowercase instance of any item
    {
        foreach (string item in test)
        {
            if (str.ToLower().StartsWith(item.ToLower()))
            {
                return true;
            }
        }

        return false;
    }

    public static string ContentOrEmtpy(this string str) //check empty string if null
    {
        if (string.IsNullOrEmpty(str))
        {
            return "";
        }

        return str;
    }

    public static int CountInstances(this string haystack, string needle)
    {
        return haystack.Select((_, i) => haystack[i..]).Count(sub => sub.StartsWith(needle));
    }

    public static int CountInstancesOfAll(this string haystack, IEnumerable<string> needles)
    {
        int instances = 0;
        foreach (string needle in needles)
        {
            instances += haystack.CountInstances(needle);
        }

        return instances;
    }

    public static string RemoveDiacritics(this string text)
    {
        string normalizedString = text.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = new StringBuilder();

        foreach (char c in normalizedString)
        {
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string ToAlphaNumeric(this string text)
    {
        return text.Unidecode();
    }


    public static void WriteAllTextFailSafe(this string text, string path, int maxAttempts = 10)
    {
        bool success = false;
        while (maxAttempts-- > 0)
        {
            try
            {
                File.WriteAllText(path, text);
                maxAttempts = -1;
            }
            catch
            {
                Console.WriteLine($"Failed to save file {path}, attempts left: {maxAttempts}");
            }
        }
    }

    public static string[] ParseArguments(this string text)
    {
        return text.Split('"')
            .Select((element, index) => index % 2 == 0  // If even index
                ? element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)  // Split the item
                : new[] { element })  // Keep the entire item
            .SelectMany(element => element).ToArray();
    }
}