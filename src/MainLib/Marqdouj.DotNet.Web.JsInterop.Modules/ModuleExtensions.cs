using System.Runtime.CompilerServices;

namespace Marqdouj.DotNet.Web.JsInterop.Modules
{
    internal enum JsModule
    {
        Geolocation,
        JsLogger,
        Observer,
    }

    internal static class ModuleExtensions
    {
        private static string GetNamespace(this JsModule jsModule)
        {
            return jsModule.ToString().Replace("_", ".");
        }

        internal static string GetJsModuleMethod(JsModule module, [CallerMemberName] string name = "")
            => $"{module.GetNamespace()}.{name.ToJsonName()}";

        /// <summary>
        /// first char must be lowercase
        /// </summary>
        internal static string ToJsonName(this string name)
        {
            var firstChar = name[0].ToString().ToLower();
            var remainder = name.Substring(1);
            return $"{firstChar}{remainder}";
        }

        internal static string? EnumToJsonN<T>(this T? value, string underscoreReplacement = "-") where T : struct, Enum
        {
            return value?.ToString().ToLower().Replace("_", underscoreReplacement);
        }

        internal static T? JsonToEnumN<T>(this string? value, string hyphenReplacement = "_", T? defaultValue = (T?)null) where T : struct, Enum
        {
            value = value?.Replace("-", hyphenReplacement);
            return Enum.TryParse(typeof(T), value, true, out var result) ? (T)result : defaultValue;
        }
    }
}
