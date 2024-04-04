using System.Security.Cryptography;
using ArcaneLibs.Extensions;

namespace ArcaneLibs;

public class SvgIdenticonGenerator {
    // based on https://github.com/stewartlord/identicon.js/blob/master/identicon.js

    public string BackgroundColor { get; set; } = "#FEFEFEFF";
    public float Saturation { get; set; } = 0.7f;
    public float Brightness { get; set; } = 0.5f;
    public string? ForegroundColor { get; set; }

    public string Generate(string identity) {
        var hash = SHA1.HashData(identity.AsBytes().ToArray());

        var hashArray = new byte[hash.Length];
        for (var i = 0; i < hash.Length; i++) {
            hashArray[i] = hash[i];
        }

        var hashInt = BitConverter.ToInt32(hashArray, 0);
        var hashIntAbs = Math.Abs(hashInt);
        var color = ForegroundColor ?? $"#{(hashIntAbs % 0xFFFFFF):X6}";

        var svg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"5\" height=\"5\" viewBox=\"0 0 5 5\">";
        svg += $"<rect x=\"0\" y=\"0\" width=\"5\" height=\"5\" fill=\"{BackgroundColor}\" />";
        for (var i = 0; i < 25; i++) {
            if ((hashIntAbs & (1 << i)) != 0) {
                var x = i % 3; // Only use the first 3 columns
                var y = (int)Math.Floor(i / 5f);
                svg += $"<rect x=\"{x}\" y=\"{y}\" width=\"1\" height=\"1\" fill=\"{color}\" />";
                svg += $"<rect x=\"{4 - x}\" y=\"{y}\" width=\"1\" height=\"1\" fill=\"{color}\" />"; // Mirror the square to the other half
            }
        }

        svg += "</svg>";
        return svg;
    }

    public string GenerateAsBase64(string identity) {
        return Convert.ToBase64String(Generate(identity).AsBytes().ToArray());
    }

    public string GenerateAsDataUri(string identity) {
        return $"data:image/svg+xml;base64,{GenerateAsBase64(identity)}";
    }
}