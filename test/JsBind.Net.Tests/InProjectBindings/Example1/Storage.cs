namespace JsBind.Net.Tests.InProjectBindings.Example1;

public abstract class Storage : ObjectBindingBase
{
    public string GetItem(string key) => Invoke<string>("getItem", key);
    public string SetItem(string key, string value) => Invoke<string>("setItem", key, value);
    public string RemoveItem(string key) => Invoke<string>("removeItem", key);
    public void Clear() => InvokeVoid("clear");
}
