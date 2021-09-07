using System.Collections.Generic;

namespace JsBind.Net.BindingConfigurations
{
    /// <summary>
    /// The binding configuration for a type.
    /// </summary>
    public class BindingConfiguration
    {
        internal static BindingConfiguration IncludeAllProperties { get; } = new()
        {
            IncludeProperties = new[] { "*" }
        };

        /// <summary>
        /// The properties to include for binding.
        /// </summary>
        public IEnumerable<string>? IncludeProperties { get; set; }

        /// <summary>
        /// The binding for each of the property that is bound.
        /// </summary>
        public IDictionary<string, BindingConfiguration?>? PropertyBindings { get; set; }

        /// <summary>
        /// The properties to exclude for binding.
        /// </summary>
        public IEnumerable<string>? ExcludeProperties { get; set; }

        /// <summary>
        /// Indicates whether the access path for this object should be set when binding.
        /// </summary>
        public bool IsBindingBase { get; set; }

        /// <summary>
        /// The binding for each of the item in the array-like value that is bound.
        /// </summary>
        public BindingConfiguration? ArrayItemBinding { get; set; }
    }
}
