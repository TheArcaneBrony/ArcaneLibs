using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;

namespace ArcaneLibs.Blazor.Components.Services;

public class BlazorSaveFileService {
    public async Task SaveFileAsync(string fileName, byte[] content, string mimeType = "text/plain") {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"))) {
            await JSHost.ImportAsync("ArcaneLibs.Blazor.Components/saveFile.js", "/_content/ArcaneLibs.Blazor.Components/saveFile.js");
        }

        await BlazorSaveFileInterop.SaveFileAsync(fileName, content, mimeType);
    }

    public async Task SaveFileAsync(string fileName, string content, string mimeType = "text/plain") {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        await SaveFileAsync(fileName, bytes, mimeType);
    }
}

internal static partial class BlazorSaveFileInterop {
    [JSImport("saveFile", "ArcaneLibs.Blazor.Components/saveFile.js")]
    public static partial Task SaveFileAsync(string fileName, byte[] content, string mimeType = "text/plain");
}