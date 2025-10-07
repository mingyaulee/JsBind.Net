using JsBind.Net.BindingConfigurations;

namespace JsBind.Net.Configurations
{
    /// <summary>
    /// The configurator for type binding.
    /// </summary>
    /// <param name="bindingConfigurationProvider">The binding configuration provider.</param>
    public class BindingConfigurator(IBindingConfigurationProvider bindingConfigurationProvider) : IBindingConfigurator
    {
        private readonly IBindingConfigurationProvider bindingConfigurationProvider = bindingConfigurationProvider;

        /// <inheritdoc />
        public ITypedBindingConfigurator Bind<T>()
            => new TypedBindingConfigurator<T>(bindingConfigurationProvider);
    }
}
