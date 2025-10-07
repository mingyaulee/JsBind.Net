using System;

namespace JsBind.Net;

/// <summary>
/// Configure this class to bind all the properties from its JavaScript object.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class BindAllPropertiesAttribute : BaseJsBindAttribute
{
    /// <summary>
    /// Configure this class to bind all the properties from its JavaScript object.
    /// </summary>
    public BindAllPropertiesAttribute()
    {
        IncludeAllProperties = true;
    }

    /// <summary>
    /// Configure this class to bind all the properties from its JavaScript object.
    /// </summary>
    /// <param name="skipBinding">If <c>true</c>, skips any binding for this class.</param>
    public BindAllPropertiesAttribute(bool skipBinding)
    {
        if (!skipBinding)
        {
            IncludeAllProperties = true;
        }
    }
}
