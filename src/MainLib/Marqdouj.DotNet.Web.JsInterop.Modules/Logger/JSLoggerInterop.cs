using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System.Runtime.CompilerServices;
using System.Text;

namespace Marqdouj.DotNet.Web.JsInterop.Modules.Logger
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IJSLogger<T> : IJSLogger where T : class
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public interface IJSLogger
    {
        /// <summary>
        /// <see cref="JSLoggerConfig"/>
        /// </summary>
        IJSLoggerConfig Config { get; set; }

        /// <summary>
        /// Flag to include full exception details when logging an exception.
        /// </summary>
        bool DetailedErrors { get; set; }

        /// <summary>
        /// <see cref="IAsyncDisposable"/>
        /// </summary>
        /// <returns></returns>
        ValueTask DisposeAsync();

        /// <summary>
        /// <see cref="ILogger.BeginScope"/>
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        IDisposable BeginScope<TState>(TState state);

        /// <summary>
        /// <see cref="ILogger.IsEnabled"/>
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        bool IsEnabled(LogLevel logLevel);

        /// <summary>
        /// Log message to the browser console.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        ValueTask Log(LogLevel logLevel, string message, string eventId = "");

        /// <summary>
        /// Log Critical message to the browser console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        ValueTask LogCritical(string message, string eventId = "");

        /// <summary>
        /// Log Debug message to the browser console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        ValueTask LogDebug(string message, string eventId = "");

        /// <summary>
        /// Log Error message to the browser console.
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        ValueTask LogError(Exception exception, string eventId = "");

        /// <summary>
        /// Log Error message to the browser console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        ValueTask LogError(string message, string eventId = "");

        /// <summary>
        /// Log Information message to the browser console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        ValueTask LogInformation(string message, string eventId = "");

        /// <summary>
        /// Log message without formatting to the browser console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        ValueTask LogRaw(string message, string style = "");

        /// <summary>
        /// Log Trace message to the browser console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        ValueTask LogTrace(string message, string eventId = "");

        /// <summary>
        /// Log Warning message to the browser console.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        ValueTask LogWarning(string message, string eventId = "");

        /// <summary>
        /// Test logging to the browser console.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        ValueTask Test(string message = "");
    }

    /// <summary>
    /// JsLogger{T} JSInterop Module
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class JSLoggerInterop<T> : JSLoggerInterop, IJSLogger<T> where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"><see cref="IJSRuntime"/></param>
        public JSLoggerInterop(IJSRuntime jsRuntime) : base(jsRuntime)
        {
            this.Config.Category = typeof(T).Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"><see cref="IJSRuntime"/></param>
        /// <param name="config"><see cref="IJSLoggerConfig"/></param>
        public JSLoggerInterop(IJSRuntime jsRuntime, IJSLoggerConfig config) : base(jsRuntime, config)
        {
            this.Config.Category = typeof(T).Name;
        }
    }

    /// <summary>
    /// JsLogger JSInterop Module
    /// </summary>
    /// <param name="jsRuntime"></param>
    public class JSLoggerInterop(IJSRuntime jsRuntime) : IAsyncDisposable, IJSLogger
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./_content/Marqdouj.DotNet.Web.JsInterop.Modules/jslogger.js").AsTask());
        private IJSLoggerConfig config = new JSLoggerConfig();

        private static readonly AsyncLocal<Stack<string?>?> _scopes = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsRuntime"><see cref="IJSRuntime"/></param>
        /// <param name="config"><see cref="IJSLoggerConfig"/></param>
        /// <exception cref="ArgumentNullException"></exception>
        public JSLoggerInterop(IJSRuntime jsRuntime, IJSLoggerConfig config) : this(jsRuntime)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// <see cref="IJSLogger.Config"/>
        /// </summary>
        public IJSLoggerConfig Config { get => config; set { ArgumentNullException.ThrowIfNull(value, nameof(Config)); config = value; } }

        /// <summary>
        /// <see cref="IJSLogger.IsEnabled(LogLevel)"/>
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel) => Config.IsEnabled(logLevel);

        /// <summary>
        /// <see cref="IJSLogger.DetailedErrors"/>
        /// </summary>
        public bool DetailedErrors { get; set; } = true;

        /// <summary>
        /// <see cref="IJSLogger.LogRaw(string, string)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public async ValueTask LogRaw(string message, string style = "")
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync(GetJsInteropMethod(), message, style);
        }

        /// <summary>
        /// <see cref="IJSLogger.LogTrace(string, string)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async ValueTask LogTrace(string message, string eventId = "")
        {
            await Log(LogLevel.Trace, message, eventId);
        }

        /// <summary>
        /// <see cref="IJSLogger.LogDebug(string, string)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async ValueTask LogDebug(string message, string eventId = "")
        {
            await Log(LogLevel.Debug, message, eventId);
        }

        /// <summary>
        /// <see cref="IJSLogger.LogInformation(string, string)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async ValueTask LogInformation(string message, string eventId = "")
        {
            await Log(LogLevel.Information, message, eventId);
        }

        /// <summary>
        /// <see cref="IJSLogger.LogWarning(string, string)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async ValueTask LogWarning(string message, string eventId = "")
        {
            await Log(LogLevel.Warning, message, eventId);
        }

        /// <summary>
        /// <see cref="IJSLogger.LogError(string, string)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async ValueTask LogError(string message, string eventId = "")
        {
            await Log(LogLevel.Error, message, eventId);
        }

        /// <summary>
        /// <see cref="IJSLogger.LogError(string, string)"/>
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async ValueTask LogError(Exception exception, string eventId = "")
        {

            var sb = new StringBuilder();
            var formattedMessage = exception.Message;

            if (!string.IsNullOrWhiteSpace(formattedMessage))
                sb.AppendLine(formattedMessage);

            if (DetailedErrors && exception != null)
                sb.AppendLine(exception.ToString());

            var message = sb.ToString();

            await Log(LogLevel.Error, message, eventId);
        }

        /// <summary>
        /// <see cref="IJSLogger.LogCritical(string, string)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async ValueTask LogCritical(string message, string eventId = "")
        {
            await Log(LogLevel.Critical, message, eventId);
        }

        /// <summary>
        /// <see cref="IJSLogger.Log(LogLevel, string, string)"/>
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        /// <param name="eventId"></param>
        /// <returns></returns>
        public async ValueTask Log(LogLevel logLevel, string message, string eventId = "")
        {
            if (!IsEnabled(logLevel))
                return;

            var logEvent = logLevel.BuildLogEventIdentifier(GetJsInteropMethod());
            var module = await moduleTask.Value;

            await module.InvokeVoidAsync(logEvent, Config, message, eventId);
        }

        /// <summary>
        /// <see cref="IJSLogger.Test(string)"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async ValueTask Test(string message = "")
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync(GetJsInteropMethod(), Config, message);
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
                    var module = await moduleTask.Value;
                    await module.DisposeAsync();
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// <see cref="ILogger.BeginScope{TState}(TState)"/>
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            _scopes.Value ??= new Stack<string?>();
            _scopes.Value.Push(state?.ToString());

            return new JSLoggerScope(() =>
            {
                _scopes.Value.Pop();
                if (_scopes.Value.Count == 0)
                {
                    _scopes.Value = null;
                }
            });
        }

        private static string GetJsInteropMethod([CallerMemberName] string name = "")
            => ModuleExtensions.GetJsModuleMethod(JsModule.JsLogger, name);
    }

    internal static class JSLoggerExtensions
    {
        internal static string BuildLogEventIdentifier(this LogLevel logLevel, string method)
        {
            string? logLevelName = logLevel switch
            {
                LogLevel.Trace => "Trace",
                LogLevel.Debug => "Debug",
                LogLevel.Information => "Information",
                LogLevel.Warning => "Warning",
                LogLevel.Error => "Error",
                LogLevel.Critical => "Critical",
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, "LogLevel not supported for logging."),
            };

            var path = $"{method}{logLevelName}";
            return path;
        }
    }
}
