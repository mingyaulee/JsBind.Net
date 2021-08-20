namespace JsBind.Net.Configurations
{
    /// <summary>
    /// The configurator for type binding.
    /// </summary>
    public interface IBindingConfigurator
    {
        /// <summary>
        /// Configure the binding for type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to configure.</typeparam>
        /// <returns>The configurator for binding of type <typeparamref name="T" />.</returns>
        IBindingConfigurator<T> Bind<T>();
    }

    /// <summary>
    /// The configurator for binding of type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type configured.</typeparam>
    public interface IBindingConfigurator<T>
    {
        /// <summary>
        /// Configure the type to include declared properties.
        /// </summary>
        void IncludeDeclaredProperties();

        /// <summary>
        /// Configure the type to include specified properties.
        /// </summary>
        /// <param name="properties">The properties to include.</param>
        void IncludeProperties(params string[] properties);

        /// <summary>
        /// Configure the type to exclude specified properties.
        /// </summary>
        /// <param name="properties">The properties to exclude.</param>
        void ExcludeProperties(params string[] properties);
    }
}
