using JsBind.Net.BindingConfigurations;

namespace JsBind.Net.Configurations;

/// <summary>
/// The options for JsBind. Use the <see cref="JsBindOptionsConfigurator" /> to configure the options.
/// </summary>
public interface IJsBindOptions
{
    internal static IJsBindOptions Instance { get; set; } = new JsBindOptions();

    /// <summary>
    /// Indicator whether to use the in process JS runtime for synchronous invocations.
    /// </summary>
    bool UseInProcessJsRuntime { get; set; }

    /// <summary>
    /// The binding configuration provider.
    /// </summary>
    IBindingConfigurationProvider BindingConfigurationProvider { get; }
}
