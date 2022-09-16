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
        ITypedBindingConfigurator Bind<T>();
    }
}
