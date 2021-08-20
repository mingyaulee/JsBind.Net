namespace JsBind.Net.Tests.InProjectBindings.Example1
{
    public class LocalStorage : Storage
    {
        public LocalStorage(IJsRuntimeAdapter jsRuntime)
        {
            SetAccessPath("localStorage");
            Initialize(jsRuntime);
        }
    }
}
