using System;

namespace JsBind.Net
{
    /// <summary>
    /// Represents the JS access path for the object, intended to be used by reflection.
    /// </summary>
    /// <param name="accessPath">The JS access path.</param>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property)]
    public class JsAccessPathAttribute(string accessPath) : Attribute
    {
        /// <summary>
        /// The JS access path.
        /// </summary>
        public string AccessPath => accessPath;
    }
}
