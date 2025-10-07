namespace JsBind.Net.Tests.InProjectBindings.Example2;

public class Storage : ObjectBindingBase
{
    public Storage(IJsRuntimeAdapter jsRuntime, string accessPath)
    {
        SetAccessPath(accessPath);
        Initialize(jsRuntime);
    }

    public string GetItem(string key) => Invoke<string>("getItem", key);
    public string SetItem(string key, string value) => Invoke<string>("setItem", key, value);
    public string RemoveItem(string key) => Invoke<string>("removeItem", key);
    public void Clear() => InvokeVoid("clear");
}
