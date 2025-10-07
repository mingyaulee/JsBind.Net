using System;

namespace JsBind.Net.Configurations;

/// <summary>
/// Configurator for JsBindOptions.
/// </summary>
public interface IJsBindOptionsConfigurator
{
    /// <summary>
    /// Use the in process JS runtime for synchronous invocations.
    /// </summary>
    /// <param name="value">Whether to use the in process JS runtime.</param>
    /// <returns>The configurator.</returns>
    IJsBindOptionsConfigurator UseInProcessJsRuntime(bool value = true);

    /// <summary>
    /// Configure the bindings.
    /// </summary>
    /// <param name="configureAction">The action to configure the bindings.</param>
    /// <returns>The configurator.</returns>
    IJsBindOptionsConfigurator ConfigureBinding(Action<IBindingConfigurator> configureAction);

    /// <summary>
    /// Gets the options configured.
    /// </summary>
    IJsBindOptions Options { get; }
}
