using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Unidecode.NET;

namespace ArcaneLibs.StringNormalisation;

public static class StringExtensions {
    public static string RemoveDiacritics(this string text) {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString) {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark) stringBuilder.Append(c);
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static string ToAlphaNumeric(this string text) => text.Unidecode();

    public static string RemoveAnsi(this string str) {
        var ansiRegex = new Regex(@"\x1B\[[0-?]*[ -/]*[@-~]");
        return ansiRegex.Replace(str, "");
    }
}
