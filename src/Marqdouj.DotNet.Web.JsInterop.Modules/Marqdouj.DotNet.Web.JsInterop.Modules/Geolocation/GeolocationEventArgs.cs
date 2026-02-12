using System.Text.Json.Serialization;

namespace Marqdouj.DotNet.Web.JsInterop.Modules.Geolocation
{
    /// <summary>
    /// Reason for the geolocation event.
    /// </summary>
    public enum GeolocationEventReason
    {
        /// <summary>
        /// Watch event was successful.
        /// </summary>
        WatchSuccess,
        /// <summary>
        /// Watch event had errors.
        /// </summary>
        WatchError,
    }

    /// <summary>
    /// Provides data for geolocation-related events, including the event type, associated key identifier, and
    /// geolocation result.
    /// </summary>
    public class GeolocationEventArgs
    {
        /// <summary>
        /// <see cref="GeolocationEventReason"/>
        /// </summary>
        [JsonIgnore]
        public GeolocationEventReason? Reason { get; internal set; }

        [JsonInclude]
        [JsonPropertyName("reason")]
        internal string? TypeJs { get => Reason.EnumToJsonN(); set => Reason = value.JsonToEnumN<GeolocationEventReason>(); }

        /// <summary>
        /// The key associated with the watch event id.
        /// </summary>
        [JsonInclude]
        public string? Key { get; internal set; }

        /// <summary>
        /// The <see cref="GeolocationResult"/> associated with the event.
        /// </summary>
        [JsonInclude]
        public GeolocationResult? Result { get; internal set; }

        /// <summary>
        /// Flag to indicate success.
        /// </summary>
        [JsonIgnore]
        public bool IsSuccess => Result is not null && Result.IsSuccess;
    }
}
