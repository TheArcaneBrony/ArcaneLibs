using Microsoft.JSInterop;

namespace ArcaneLibs.Blazor.Components.Services;

public class JsConsoleService(IJSRuntime jsRuntime) {
    public async Task Log(params object[] args) => await jsRuntime.InvokeVoidAsync("console.log", args);
    public async Task Info(params object[] args) => await jsRuntime.InvokeVoidAsync("console.info", args);
    public async Task Debug(params object[] args) => await jsRuntime.InvokeVoidAsync("console.debug", args);
    public async Task Warn(params object[] args) => await jsRuntime.InvokeVoidAsync("console.warn", args);
    public async Task Error(params object[] args) => await jsRuntime.InvokeVoidAsync("console.error", args);
    public async Task Dir(params object[] args) => await jsRuntime.InvokeVoidAsync("console.dir", args);
    public async Task Time(params object[] args) => await jsRuntime.InvokeVoidAsync("console.time", args);
    public async Task TimeEnd(params object[] args) => await jsRuntime.InvokeVoidAsync("console.timeEnd", args);
    public async Task TimeLog(params object[] args) => await jsRuntime.InvokeVoidAsync("console.timeLog", args);
    public async Task Trace(params object[] args) => await jsRuntime.InvokeVoidAsync("console.trace", args);
    public async Task Assert(params object[] args) => await jsRuntime.InvokeVoidAsync("console.assert", args);
    public async Task Clear(params object[] args) => await jsRuntime.InvokeVoidAsync("console.clear", args);
    public async Task Count(params object[] args) => await jsRuntime.InvokeVoidAsync("console.count", args);
    public async Task CountReset(params object[] args) => await jsRuntime.InvokeVoidAsync("console.countReset", args);
    public async Task Group(params object[] args) => await jsRuntime.InvokeVoidAsync("console.group", args);
    public async Task GroupEnd(params object[] args) => await jsRuntime.InvokeVoidAsync("console.groupEnd", args);
    public async Task Table(params object[] args) => await jsRuntime.InvokeVoidAsync("console.table", args);
    public async Task Dirxml(params object[] args) => await jsRuntime.InvokeVoidAsync("console.dirxml", args);
    public async Task GroupCollapsed(params object[] args) => await jsRuntime.InvokeVoidAsync("console.groupCollapsed", args);
    public async Task Profile(params object[] args) => await jsRuntime.InvokeVoidAsync("console.profile", args);
    public async Task ProfileEnd(params object[] args) => await jsRuntime.InvokeVoidAsync("console.profileEnd", args);
    public async Task TimeStamp(params object[] args) => await jsRuntime.InvokeVoidAsync("console.timeStamp", args);
    public async Task Context(params object[] args) => await jsRuntime.InvokeVoidAsync("console.context", args);
    public async Task CreateTask(params object[] args) => await jsRuntime.InvokeVoidAsync("console.createTask", args);
}