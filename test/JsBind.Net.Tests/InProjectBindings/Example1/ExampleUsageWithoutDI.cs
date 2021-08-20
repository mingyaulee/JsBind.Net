using JsBind.Net.Configurations;
using Microsoft.JSInterop;

namespace JsBind.Net.Tests.InProjectBindings.Example1
{
    public abstract class ExampleUsageWithoutDI
    {
        public void UseStorage()
        {
            var options = new JsBindOptionsConfigurator()
                .UseInProcessJsRuntime()
                .Options;
            var jsRuntimeAdapter = new JsRuntimeAdapter(JsRuntime, options);
            var localStorage = new LocalStorage(jsRuntimeAdapter);
            localStorage.SetItem("storageKey", "storageValue");
        }

        public abstract IJSRuntime JsRuntime { get; }
    }
}
