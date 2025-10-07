namespace JsBind.Net.Tests.InProjectBindings.Example2
{
    public class Global(IJsRuntimeAdapter jsRuntime)
    {
        public Storage LocalStorage { get; } = new Storage(jsRuntime, "localStorage");
        public Storage SessionStorage { get; } = new Storage(jsRuntime, "sessionStorage");
    }
}
