using JsBind.Net.Configurations;
using Microsoft.JSInterop;

namespace JsBind.Net.Internal
{
    /// <summary>
    /// Proxy adapter for JS runtime. Used for deserialization when the real JS runtime is not available yet.
    /// </summary>
    internal class ProxyJsRuntimeAdapter : JsRuntimeAdapter
    {
        private JsRuntimeAdapter? jsRuntime;

        public override IJsBindOptions JsBindOptions => jsRuntime!.JsBindOptions;
        public override IJSRuntime JsRuntime => jsRuntime!.JsRuntime;

        public void SetJsRuntime(JsRuntimeAdapter jsRuntime) => this.jsRuntime = jsRuntime;
    }
}
