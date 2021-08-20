using JsBind.Net;

namespace TestBindings.Server.BindingTestLibrary
{
    public class TestBoundClass : ObjectBindingBase<TestBoundClass>
    {
        public double RandomValue { get; set; }
    }
}
