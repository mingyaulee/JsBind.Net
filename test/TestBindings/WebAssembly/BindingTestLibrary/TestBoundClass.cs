using JsBind.Net;

namespace TestBindings.WebAssembly.BindingTestLibrary
{
    public class TestBoundClass : ObjectBindingBase<TestBoundClass>
    {
        public double RandomValue { get; set; }
    }
}
