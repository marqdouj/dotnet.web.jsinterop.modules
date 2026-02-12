namespace Marqdouj.DotNet.Web.JsInterop.Modules.Geolocation
{
    /// <summary>
    /// The reason for a Geolocation error, based on <see href="https://developer.mozilla.org/en-US/docs/Web/API/GeolocationPositionError"/>.
    /// </summary>
    public class GeolocationPositionError
    {
        /// <summary>
        /// <see href="https://developer.mozilla.org/en-US/docs/Web/API/GeolocationPositionError/code"/>
        /// </summary>
        public int? Code { get; set; }

        /// <summary>
        /// Details of the error.
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// <see cref="object.ToString"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Message ?? "";
        }
    }
}
