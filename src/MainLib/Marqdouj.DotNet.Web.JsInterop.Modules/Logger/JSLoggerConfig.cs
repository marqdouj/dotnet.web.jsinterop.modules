using Microsoft.Extensions.Logging;

namespace Marqdouj.DotNet.Web.JsInterop.Modules.Logger
{
    /// <summary>
    /// 
    /// </summary>
    public interface IJSLoggerConfig : ICloneable
    {
        /// <summary>
        /// Default template to use for logging when a template is not speficied.
        /// </summary>
        string DefaultTemplate { get; set; }

        /// <summary>
        /// Category template value.
        /// </summary>
        string Category { get; set; }

        /// <summary>
        /// Checks if logging is enabled for the specified LogLevel.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        bool IsEnabled(LogLevel logLevel);

        /// <summary>
        /// Maximum level for logging.
        /// </summary>
        LogLevel MaxLevel { get; }

        /// <summary>
        /// Minimum level for logging.
        /// </summary>
        LogLevel MinLevel { get; }

        /// <summary>
        /// Template to use for logging.
        /// </summary>
        string Template { get; set; }

        /// <summary>
        /// Sets the min/max log level.
        /// </summary>
        /// <param name="min"><see cref="MinLevel"/></param>
        /// <param name="max"><see cref="MaxLevel"/></param>
        void SetLevel(LogLevel min, LogLevel max);
    }

    /// <summary>
    /// JsLogger Configuration
    /// </summary>
    public class JSLoggerConfig : IJSLoggerConfig
    {
        private static string defaultTemplate = "{category}{event}{timestamp}{level}: {message}";
        private string template = defaultTemplate;

        /// <summary>
        /// <see cref="IJSLoggerConfig.DefaultTemplate"/>
        /// </summary>
        public string DefaultTemplate
        {
            get => defaultTemplate;
            set
            {
                ArgumentNullException.ThrowIfNullOrWhiteSpace(value, nameof(defaultTemplate));
                defaultTemplate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"><see cref="IJSLoggerConfig.Category"/></param>
        /// <param name="min"><see cref="IJSLoggerConfig.MinLevel"/></param>
        /// <param name="max"><see cref="IJSLoggerConfig.MaxLevel"/></param>
        /// <param name="template"><see cref="IJSLoggerConfig.Template"/></param>
        public JSLoggerConfig(string? category = null, LogLevel min = LogLevel.Information, LogLevel max = LogLevel.Critical, string template = "")
        {
            Category = category ?? nameof(JSLoggerInterop);
            Template = string.IsNullOrWhiteSpace(template) ? DefaultTemplate : template;
            SetLevel(min, max);
        }

        /// <summary>
        /// <see cref="IJSLoggerConfig.Category"/>
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// <see cref="IJSLoggerConfig.MinLevel"/>
        /// </summary>
        public LogLevel MinLevel { get; private set; } = LogLevel.Information;

        /// <summary>
        /// <see cref="IJSLoggerConfig.MaxLevel"/>
        /// </summary>
        public LogLevel MaxLevel { get; private set; } = LogLevel.Critical;

        /// <summary>
        /// <see cref="IJSLoggerConfig.Template"/>
        /// </summary>
        public string Template { get => template; set { ArgumentNullException.ThrowIfNullOrWhiteSpace(value, nameof(Template)); template = value; } }

        /// <summary>
        /// <see cref="IJSLoggerConfig.IsEnabled(LogLevel)"/>
        /// </summary>
        /// <param name="logLevel"><see cref="LogLevel"/></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None && logLevel >= MinLevel && logLevel <= MaxLevel;
        }

        /// <summary>
        /// <see cref="IJSLoggerConfig.SetLevel(LogLevel, LogLevel)"/>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetLevel(LogLevel min, LogLevel max)
        {
            if (min > max)
            {
                throw new ArgumentException($"Minimum log level {min} cannot be greater than maximum log level {max}.");
            }
            MinLevel = min;
            MaxLevel = max;
        }

        /// <summary>
        /// <see cref="ICloneable"/>
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
