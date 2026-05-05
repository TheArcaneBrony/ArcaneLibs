using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace ArcaneLibs.Blazor.Components.Services;

public class BaseStorageService(ILogger<BaseStorageService> logger, IJSRuntime jsRuntime, string storageName) {
    public async Task<string?> GetItemAsync(string name) => await jsRuntime.InvokeAsync<string?>(storageName + ".getItem", name);
    public async Task SetItemAsync(string name, string data) => await jsRuntime.InvokeVoidAsync(storageName + ".setItem", name, data);
    public async Task RemoveItemAsync(string name) => await jsRuntime.InvokeVoidAsync(storageName + ".removeItem", name);
    public async Task ClearAsync() => await jsRuntime.InvokeVoidAsync(storageName + ".clear");
    public async Task<bool> ContainsKeyAsync(string key) => await jsRuntime.InvokeAsync<bool>(storageName + ".hasOwnProperty", key);
    public async Task SetItemAsJsonAsync<T>(string name, T data) => await jsRuntime.InvokeVoidAsync(storageName + ".setItem", name, JsonSerializer.Serialize(data));

    public async Task<T?> GetItemFromJsonAsync<T>(string name) {
        var json = await jsRuntime.InvokeAsync<string?>(storageName + ".getItem", name);
        logger.LogError("Got item from json: " + (json ?? "(actual null)"));
        return string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json);
    }
}

public class LocalStorageService(ILogger<LocalStorageService> logger, IJSRuntime jsRuntime) : BaseStorageService(logger, jsRuntime, "window.localStorage");

public class SessionStorageService(ILogger<SessionStorageService> logger, IJSRuntime jsRuntime) : BaseStorageService(logger, jsRuntime, "window.sessionStorage");