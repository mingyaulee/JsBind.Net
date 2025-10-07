using System;
using System.Collections.Generic;

namespace JsBind.Net.BindingConfigurations;

/// <summary>
/// The provider of binding configuration.
/// </summary>
public interface IBindingConfigurationProvider
{
    /// <summary>
    /// Gets the binding configuration of the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The binding configuration.</returns>
    BindingConfiguration? Get(Type type);

    /// <summary>
    /// Adds the binding configuration for the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="bindingConfiguration">The binding configuration.</param>
    void Add(Type type, BindingConfiguration? bindingConfiguration);

    /// <summary>
    /// Gets the declared properties on the type.
    /// </summary>
    /// <param name="type">The type.</param>
    IEnumerable<string> GetDeclaredProperties(Type type);
}
