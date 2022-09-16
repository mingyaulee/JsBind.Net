using JsBind.Net.BindingConfigurations;

namespace JsBind.Net.Configurations
{
    /// <summary>
    /// The configurator for type binding.
    /// </summary>
    public class BindingConfigurator : IBindingConfigurator
    {
        private readonly IBindingConfigurationProvider bindingConfigurationProvider;

        /// <summary>
        /// Creates a new instance of <see cref="BindingConfigurator" />.
        /// </summary>
        /// <param name="bindingConfigurationProvider">The binding configuration provider.</param>
        public BindingConfigurator(IBindingConfigurationProvider bindingConfigurationProvider)
        {
            this.bindingConfigurationProvider = bindingConfigurationProvider;
        }

        /// <inheritdoc />
        public ITypedBindingConfigurator Bind<T>()
        {
            return new TypedBindingConfigurator<T>(bindingConfigurationProvider);
        }
    }
}
