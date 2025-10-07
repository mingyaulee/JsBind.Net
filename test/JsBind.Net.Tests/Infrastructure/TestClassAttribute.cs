using System;

namespace JsBind.Net.Tests.Infrastructure;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class TestClassAttribute : Attribute
{
    public string Description { get; set; }
}
