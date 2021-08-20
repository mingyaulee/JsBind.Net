using System;

namespace JsBind.Net
{
    /// <summary>
    /// Configure this class to bind all the properties without the excluded properties from its JavaScript object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BindExcludePropertiesAttribute : BaseJsBindAttribute
    {
        /// <summary>
        /// Configure this class to bind all the properties without the excluded properties from its JavaScript object.
        /// </summary>
        public BindExcludePropertiesAttribute(params string[] excludeProperties)
        {
            ExcludeProperties = excludeProperties;
        }
    }
}
