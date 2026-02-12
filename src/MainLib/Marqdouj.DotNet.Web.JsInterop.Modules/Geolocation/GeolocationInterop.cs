using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Marqdouj.DotNet.Web.JsInterop.Modules.Geolocation
{
    /// <summary>
    /// Geolocation JSInterop Module
    /// </summary>
    public class GeolocationInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly DotNetObjectReference<GeolocationInterop>? dotNetRef;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"></param>
        [DynamicDependency(nameof(NotifyGeolocationWatch))]
        public GeolocationInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/Marqdouj.DotNet.Web.JsInterop.Modules/geolocation.js").AsTask());
            dotNetRef = DotNetObjectReference.Create(this);
        }

        /// <summary>
        /// Sets the LogLevel for interop module.
        /// </summary>
        /// <param name="level"><see cref="LogLevel"/></param>
        /// <returns></returns>
        public async ValueTask SetLogLevel(LogLevel level)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync(GetJsInteropMethod(), level);
        }

        /// <summary>
        /// Get the current Geolocation.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async ValueTask<GeolocationResult> GetLocation(PositionOptions? options = null)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<GeolocationResult>(GetJsInteropMethod(), options);
        }

        /// <summary>
        /// Adds a geolocation watch for the specified key.
        /// </summary>
        /// <param name="key"><see cref="GeolocationEventArgs.Key"/></param>
        /// <param name="options"></param>
        /// <returns>The id that identifies the registered handler associated with the <see cref="GeolocationEventArgs.Key"/></returns>
        public async ValueTask<int?> WatchPosition(string key, PositionOptions? options = null)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<int?>(GetJsInteropMethod(), dotNetRef, key, options);
        }

        /// <summary>
        /// Clears the watch associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async ValueTask ClearWatch(string key)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync(GetJsInteropMethod(), key);
        }

        /// <summary>
        /// Clears all watches.
        /// </summary>
        /// <returns></returns>
        public async ValueTask ClearWatches()
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync(GetJsInteropMethod());
        }

        /// <summary>
        /// Checks if a specified key is being watched.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async ValueTask<bool> IsWatched(string key)
        {
            var module = await moduleTask.Value;
            return await module.InvokeAsync<bool>(GetJsInteropMethod(), key);
        }

        /// <summary>
        /// Subscribe to this action to receive geolocation watch notifications.
        /// </summary>
        public event Action<GeolocationEventArgs>? OnGeolocationWatch;

        /// <summary>
        /// Subscribe to this action to receive async geolocation watch notifications.
        /// </summary>
        public Func<GeolocationEventArgs, Task>? OnGeolocationWatchAsync { get; set; }

        /// <summary>
        /// <see cref="JSInvokableAttribute"/> Handler associated with this module.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [JSInvokable]
        public async Task NotifyGeolocationWatch(GeolocationEventArgs e)
        {
            if (OnGeolocationWatchAsync != null)
                await OnGeolocationWatchAsync.Invoke(e);

            OnGeolocationWatch?.Invoke(e);
        }

        /// <summary>
        /// <see cref="IAsyncDisposable"/>
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                try
                {
                    await ClearWatches();
                }
                catch (Exception) { }

                try
                {
                    var module = await moduleTask.Value;
                    await module.DisposeAsync();
                }
                catch (Exception) { }
            }

            dotNetRef?.Dispose();
        }

        private static string GetJsInteropMethod([CallerMemberName] string name = "")
            => ModuleExtensions.GetJsModuleMethod(JsModule.Geolocation, name);
    }
}
