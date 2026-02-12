using System.Text.Json.Serialization;

namespace Marqdouj.DotNet.Web.JsInterop.Modules.Observer
{
    /// <summary>
    /// Arguments returned from the module ResizeObserver event.
    /// </summary>
    public class OnObserveResizedArgs
    {
        /// <summary>
        /// Element id being observed.
        /// </summary>
        [JsonInclude] public string? Id { get; internal set; }

        /// <summary>
        /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserverSize"/>
        /// </summary>
        [JsonInclude] public double? Height { get; internal set; }

        /// <summary>
        /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserverSize"/>
        /// </summary>
        [JsonInclude] public double? Width { get; internal set; }
    }
}