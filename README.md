# DotNet.Web.JsInterop.Modules

## Summary
[JSInterop](https://learn.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability) modules 
I find useful in my [Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/) projects.
These modules are self-contained (script is loaded/unloaded dynamically; no need to add scripts to App.Razor).

## Demo
A demo of this library, and some of my other `DotNet` libraries, can be found [here](https://github.com/marqdouj/dotnet.demo).

## Modules
- `GeolocationInterop`.
  - JSInterop Module that works with the [Geolocation API](https://developer.mozilla.org/en-US/docs/Web/API/Geolocation).
- `ObserverInterop`.
  - JSInterop Module that works with the [ResizeObserver API](https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserverSize).

## Release Notes
- `10.0.0`
  - Initial release.
