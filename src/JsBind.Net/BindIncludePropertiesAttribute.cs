using System;

namespace JsBind.Net
{
    /// <summary>
    /// Configure this class to bind only the included properties from its JavaScript object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BindIncludePropertiesAttribute : BaseJsBindAttribute
    {
        /// <summary>
        /// Configure this class to bind only the included properties from its JavaScript object.
        /// </summary>
        public BindIncludePropertiesAttribute(params string[] includeProperties)
        {
            IncludeProperties = includeProperties;
        }
    }
}
