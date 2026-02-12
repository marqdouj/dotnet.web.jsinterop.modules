using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Marqdouj.DotNet.Web.JsInterop.Modules.Observer
{
    /// <summary>
    /// Observer JSInterop Module
    /// </summary>
    public class ObserverInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly DotNetObjectReference<ObserverInterop>? dotNetRef;

        /// <param name="jsRuntime"></param>
        [DynamicDependency(nameof(NotifyObserveResized))]
        public ObserverInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/Marqdouj.DotNet.Web.JsInterop.Modules/observer.js").AsTask());
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
        /// <see cref="AddResizers(List{string})"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask AddResizer(string id)
        {
            await AddResizers([id]);
        }

        /// <summary>
        /// Add elements to ResizeObserver <see href="https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserver"/>
        /// </summary>
        /// <param name="ids">List of element IDs</param>
        /// <returns></returns>
        public async ValueTask AddResizers(List<string> ids)
        {
            var module = await moduleTask.Value;
            await module.InvokeAsync<string>(GetJsInteropMethod(), dotNetRef, ids);
        }

        /// <summary>
        /// <see cref="RemoveResizers(List{string})"/>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask RemoveResizer(string id)
        {
            await RemoveResizers([id]);
        }

        /// <summary>
        /// Remove elements from ResizeObserver <see href="https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserver"/>
        /// </summary>
        /// <param name="ids">List of element IDs</param>
        /// <returns></returns>
        public async ValueTask RemoveResizers(List<string> ids)
        {
            var module = await moduleTask.Value;
            await module.InvokeAsync<string>(GetJsInteropMethod(), ids);
            foreach (var id in ids)
                ids.Remove(id);
        }

        /// <summary>
        /// Disconnects ResizeObserver.
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisconnectResizers()
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync(GetJsInteropMethod());
        }

        /// <summary>
        /// Subscribe to this action to receive notifications when element size changes.
        /// </summary>
        public event Action<OnObserveResizedArgs>? OnObserveResized;

        /// <summary>
        /// Subscribe to this action to receive async notifications when element size changes.
        /// </summary>
        public Func<OnObserveResizedArgs, Task>? OnObserveResizedAsync { get; set; }

        /// <summary>
        /// <see cref="JSInvokableAttribute"/> Handler associated with this module.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [JSInvokable]
        public async Task NotifyObserveResized(OnObserveResizedArgs args)
        {
            if (OnObserveResizedAsync != null)
                await OnObserveResizedAsync.Invoke(args);

            OnObserveResized?.Invoke(args);
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
                    await DisconnectResizers();
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
            => ModuleExtensions.GetJsModuleMethod(JsModule.Observer, name);

    }
}
