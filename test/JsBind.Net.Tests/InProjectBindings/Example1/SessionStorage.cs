namespace JsBind.Net.Tests.InProjectBindings.Example1;

public class SessionStorage : Storage
{
    public SessionStorage(IJsRuntimeAdapter jsRuntime)
    {
        SetAccessPath("sessionStorage");
        Initialize(jsRuntime);
    }
}
