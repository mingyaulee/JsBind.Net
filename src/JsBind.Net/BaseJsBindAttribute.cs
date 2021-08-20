using System;
using System.Collections.Generic;

namespace JsBind.Net
{
    /// <summary>
    /// Base attribute for binding configuration.
    /// </summary>
    public abstract class BaseJsBindAttribute : Attribute
    {
        internal IEnumerable<string>? IncludeProperties { get; set; }
        internal IEnumerable<string>? ExcludeProperties { get; set; }
        internal bool IncludeAllProperties { get; set; }
        internal bool IncludeDeclaredPropertiesOnly { get; set; }
    }
}
