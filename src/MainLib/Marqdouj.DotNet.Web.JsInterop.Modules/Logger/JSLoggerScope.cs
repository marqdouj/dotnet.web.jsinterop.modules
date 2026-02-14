namespace Marqdouj.DotNet.Web.JsInterop.Modules.Logger
{
    internal class JSLoggerScope(Action onDispose) : IDisposable
    {
        private readonly Action _onDispose = onDispose;

        public void Dispose()
        {
            _onDispose();
        }
    }
}
