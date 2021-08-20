using System;

namespace JsBind.Net.Configurations
{
    /// <summary>
    /// Configurator for <see cref="IJsBindOptions" />.
    /// </summary>
    public class JsBindOptionsConfigurator : IJsBindOptionsConfigurator
    {
        private readonly IJsBindOptions options;

        /// <summary>
        /// Creates a new instace of <see cref="JsBindOptionsConfigurator" /> to configure JsBindOptions.
        /// </summary>
        public JsBindOptionsConfigurator()
        {
            options = IJsBindOptions.Instance;
        }

        /// <inheritdoc />
        public IJsBindOptionsConfigurator UseInProcessJsRuntime(bool value = true)
        {
            options.UseInProcessJsRuntime = value;
            return this;
        }

        /// <inheritdoc />
        public IJsBindOptionsConfigurator ConfigureBinding(Action<IBindingConfigurator> configureAction)
        {
            var configurator = new BindingConfigurator(options.BindingConfigurationProvider);
            configureAction.Invoke(configurator);
            return this;
        }

        /// <inheritdoc />
        public IJsBindOptions Options => options;
    }
}
