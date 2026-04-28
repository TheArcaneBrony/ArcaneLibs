using System.Text.Json;
using Microsoft.JSInterop;

namespace ArcaneLibs.Blazor.Components.Services;

public class BaseStorageService(IJSRuntime jsRuntime, string storageName) {
    public async Task SetItemAsync(string name, string data) => await jsRuntime.InvokeVoidAsync(storageName + ".setItem", name, data);
    public async Task RemoveItemAsync(string name) => await jsRuntime.InvokeVoidAsync(storageName + ".removeItem", name);
    public async Task ClearAsync() => await jsRuntime.InvokeVoidAsync(storageName + ".clear");
    public async Task SetItemAsJsonAsync<T>(string name, T data) => await jsRuntime.InvokeVoidAsync(storageName + ".setItem", name, data);
    public async Task<T?> GetItemFromJsonAsync<T>(string name) {
        var json = await jsRuntime.InvokeAsync<string>(storageName + ".getItem", name);
        return JsonSerializer.Deserialize<T>(json);
    }
}

public class LocalStorageService(IJSRuntime jsRuntime) : BaseStorageService(jsRuntime, "window.localStorage");
public class SessionStorageService(IJSRuntime jsRuntime) : BaseStorageService(jsRuntime, "window.sessionStorage");