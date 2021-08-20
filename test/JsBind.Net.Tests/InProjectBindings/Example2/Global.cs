namespace JsBind.Net.Tests.InProjectBindings.Example2
{
    public class Global
    {
        public Global(IJsRuntimeAdapter jsRuntime)
        {
            LocalStorage = new Storage(jsRuntime, "localStorage");
            SessionStorage = new Storage(jsRuntime, "sessionStorage");
        }

        public Storage LocalStorage { get; }
        public Storage SessionStorage { get; }
    }
}
