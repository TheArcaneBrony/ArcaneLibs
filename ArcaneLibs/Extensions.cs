using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Unidecode.NET;

namespace ArcaneLibs
{
    public static class Extensions
    {
        public static bool ContainsAnyCase(this string str, string test) //check if lowercase string contains variable "test" (should be lowercase, otherwise always returns false)
        {
            return str.ToLower().Contains(test);
        }

        public static bool ContainsAnyOf(this string str, IEnumerable<string> test) //check if string contains any lowercase instance of any item
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

        public static bool MatchesAny(this string str, IEnumerable<string> regex, RegexOptions options) //check if string contains any lowercase instance of any item
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

        public static bool StartsWithAnyOf(this string str, IEnumerable<string> test) //check if string starts with any lowercase instance of any item
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

        public static void SaveToJsonFile(this object @object, string filename) // save object to json file
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                DefaultValueHandling = DefaultValueHandling.Populate,
            };
            settings.Converters.Add(new StringEnumConverter());
            // serialise object
            string json = JsonConvert.SerializeObject(@object, Formatting.Indented, settings);
            // save to files
            try
            {
                Util.WriteAllTextIfDifferent(filename, json);
                // using (var stream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                // {
                //     StreamWriter w = new StreamWriter(stream);
                //         w.Write(json);
                //         w.Flush();
                //         w.Close();
                //     stream.Flush();
                //     stream.Close();
                // }
            }
            catch
            {
                // ignored
            }
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

        public static string Toggle(this ref bool @bool)
        {
            return (@bool ^= true) ? "enabled" : "disabled";
        }

        public static (bool, string) ToggleAlt(this bool @bool)
        {
            return (@bool ^= true, @bool ? "enabled" : "disabled");
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

        public static void RemoveAll<K, V>(this IDictionary<K, V> dict, Func<K, V, bool> match)
        {
            foreach (var key in dict.Keys.ToArray()
                         .Where(key => match(key, dict[key])))
                dict.Remove(key);
        }

        public static T[] AddToArray<T>(this T[] array, T item)
        {
            Array.Resize(ref array, array.Length + 1);
            array[^1] = item;
            return array;
        }
        
        public static string TimeSinceFormatted(this DateTimeOffset dto)
        {
            TimeSpan ts = DateTime.Now - dto;
            return ts.TimeSinceFormatted();
        }
        public static string TimeSinceFormatted(this TimeSpan timeSpan)
        {
            string formatted = "";
            if (timeSpan.Days > 0) formatted += $"{timeSpan.Days}.";
            if (timeSpan.TotalHours > 0) formatted += $"{timeSpan.Hours:00}:";
            if (timeSpan.TotalMinutes > 0) formatted += $"{timeSpan.Minutes:00}{(timeSpan.Seconds%2==1?".":":")}";
            if (timeSpan.TotalSeconds > 0) formatted += $"{timeSpan.Seconds:00}";
            return formatted;
        }
    }
}