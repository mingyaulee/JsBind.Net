using System;

namespace JsBind.Net;

/// <summary>
/// Configure this class to bind only the declared properties from its JavaScript object.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class BindDeclaredPropertiesAttribute : BaseJsBindAttribute
{
    /// <summary>
    /// Configure this class to bind only the declared properties from its JavaScript object.
    /// </summary>
    public BindDeclaredPropertiesAttribute()
    {
        IncludeDeclaredPropertiesOnly = true;
    }
}
