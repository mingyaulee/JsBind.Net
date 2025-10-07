using System.Text.Json.Serialization;
using JsBind.Net;

namespace TestBindings.Server.BindingTestLibrary;

public class TestBoundClass : ObjectBindingBase
{
    [JsonPropertyName("isTestClass")]
    public bool IsTestClass { get; set; }
    public double RandomValue { get; set; }
}
