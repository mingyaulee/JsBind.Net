using JsBind.Net.BindingConfigurations;

namespace JsBind.Net.Configurations
{
    /// <summary>
    /// The options for JsBind.
    /// </summary>
    internal class JsBindOptions : IJsBindOptions
    {
        /// <summary>
        /// Indicator whether to use the in process JS runtime for synchronous invocations.
        /// </summary>
        public bool UseInProcessJsRuntime { get; set; }

        /// <summary>
        /// The binding configuration provider.
        /// </summary>
        public IBindingConfigurationProvider BindingConfigurationProvider { get; } = new BindingConfigurationProvider();
    }
}
