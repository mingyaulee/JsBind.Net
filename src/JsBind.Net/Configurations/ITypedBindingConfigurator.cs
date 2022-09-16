namespace JsBind.Net.Configurations
{
    /// <summary>
    /// The configurator for binding of a defined type.
    /// </summary>
    public interface ITypedBindingConfigurator
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
