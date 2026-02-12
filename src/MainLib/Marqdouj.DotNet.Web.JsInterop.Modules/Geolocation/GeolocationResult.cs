using System.Text.Json.Serialization;

namespace Marqdouj.DotNet.Web.JsInterop.Modules.Geolocation
{
    /// <summary>
    /// The result of a geolocation request. Contains either a <see cref="GeolocationPosition"/> or a <see cref="GeolocationPositionError"/>.
    /// </summary>
    public class GeolocationResult
    {
        /// <summary>
        /// The <see cref="GeolocationPosition"/> returned on successful geolocation.
        /// </summary>
        public GeolocationPosition? Position { get; set; }
        /// <summary>
        /// The <see cref="GeolocationPositionError"/> returned by a failed geolocation attempt.
        /// </summary>
        public GeolocationPositionError? Error { get; set; }

        /// <summary>
        /// Indicates whether the geolocation attempt was successful.
        /// </summary>
        [JsonIgnore]
        public bool IsSuccess => Position != null;

        /// <summary>
        /// <see cref="object.ToString"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Sucess:{IsSuccess} Position:{Position} Error:{Error}";
        }
    }
}
