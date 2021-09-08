using System;

namespace JsBind.Net
{
    /// <summary>
    /// Ignore this property for binding.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class BindIgnoreAttribute : Attribute
    {
    }
}
