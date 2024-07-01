using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ArcaneLibs.Blazor.Components;

public static class StreamExtensions {
    private static IJSObjectReference? _streamExtensionsModule;
    
    // public static async Task<string> GetBlobUriAsync(this IJSRuntime jsRuntime, Stream stream, bool revokeAfterFinish = true, int revocationDelay = 1000) {
    //     if (_streamExtensionsModule is null) {
    //         Console.WriteLine("Importing stream extensions module");
    //         _streamExtensionsModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/ArcaneLibs.Blazor.Components/streamExtensions.js");
    //     }
    //  
    //     Console.WriteLine($"Blob stream {stream.GetHashCode()} position {stream.Position} length {stream.Length}");
    //     if (revokeAfterFinish) {
    //         Task.Run(async () => {
    //             var isDisposed = false;
    //             long lastPosition = 0;
    //             while (!isDisposed) {
    //                 try {
    //                     await Task.Delay(revocationDelay);
    //                     Console.WriteLine($"Blob stream {stream.GetHashCode()} position {stream.Position} length {stream.Length}");
    //                 }
    //                 catch (ObjectDisposedException) {
    //                     isDisposed = true;
    //                 }
    //             }
    //             
    //             Console.WriteLine($"Blob stream {stream.GetHashCode()} disposed, revoking after {lastPosition} bytes!");
    //             await _streamExtensionsModule.InvokeVoidAsync("revokeBlobUri", stream);
    //         });
    //     }
    //
    //     using var streamRef = new DotNetStreamReference(stream, true);
    //     Console.WriteLine("got streamRef");
    //     return await _streamExtensionsModule.InvokeAsync<string>("getBlobUri", streamRef);
    // }
    
    
    public static async Task<string> streamImage(this IJSRuntime jsRuntime, Stream stream, ElementReference element, bool revokeAfterFinish = false, int revocationDelay = 1000) {
        if (_streamExtensionsModule is null) {
            Console.WriteLine("Importing stream extensions module");
            _streamExtensionsModule = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/ArcaneLibs.Blazor.Components/streamExtensions.js");
            Console.WriteLine("Imported stream extensions module");
        }
     
        Console.WriteLine($"Blob stream {stream.GetHashCode()}");
        if (revokeAfterFinish) {
            Task.Run(async () => {
                var isDisposed = false;
                long lastPosition = 0;
                while (!isDisposed) {
                    try {
                        await Task.Delay(revocationDelay);
                        Console.WriteLine($"Blob stream {stream.GetHashCode()} position {stream.Position} length {stream.Length}");
                    }
                    catch (ObjectDisposedException) {
                        isDisposed = true;
                    }
                }
                
                Console.WriteLine($"Blob stream {stream.GetHashCode()} disposed, revoking after {lastPosition} bytes!");
                await _streamExtensionsModule.InvokeVoidAsync("revokeBlobUri", stream);
            });
        }

        using var streamRef = new DotNetStreamReference(stream, true);
        Console.WriteLine("got streamRef");
        return await _streamExtensionsModule.InvokeAsync<string>("streamImage", streamRef, element);
    }

}