using System.Collections.Generic;
using System.Linq;
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
        public IBindingConfigurator<T> Bind<T>()
        {
            return new BindingConfigurator<T>(bindingConfigurationProvider);
        }
    }

    /// <summary>
    /// The configurator for binding of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type configured.</typeparam>
    public class BindingConfigurator<T> : IBindingConfigurator<T>
    {
        private readonly IBindingConfigurationProvider bindingConfigurationProvider;
        private BindingConfiguration? bindingConfiguration;
        private BindingConfiguration BindingConfiguration
        {
            get
            {
                if (bindingConfiguration is null)
                {
                    bindingConfiguration = new BindingConfiguration();
                    bindingConfigurationProvider.Add(typeof(T), bindingConfiguration);
                }

                return bindingConfiguration;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="BindingConfigurator" />.
        /// </summary>
        /// <param name="bindingConfigurationProvider">The binding configuration provider.</param>
        public BindingConfigurator(IBindingConfigurationProvider bindingConfigurationProvider)
        {
            this.bindingConfigurationProvider = bindingConfigurationProvider;
        }

        /// <inheritdoc />
        public void IncludeDeclaredProperties()
        {
            AddIncludeProperties(bindingConfigurationProvider.GetDeclaredProperties(typeof(T)));
        }

        /// <inheritdoc />
        public void IncludeProperties(params string[] properties)
        {
            AddIncludeProperties(properties);
        }

        /// <inheritdoc />
        public void ExcludeProperties(params string[] properties)
        {
            BindingConfiguration.ExcludeProperties = properties;
        }

        private void AddIncludeProperties(IEnumerable<string> includeProperties)
        {
            if (BindingConfiguration.IncludeProperties is null)
            {
                BindingConfiguration.IncludeProperties = includeProperties;
            }
            else
            {
                BindingConfiguration.IncludeProperties = BindingConfiguration.IncludeProperties.Concat(includeProperties).ToList();
            }
        }
    }
}
