namespace JsBind.Net.Tests.InProjectBindings.Example1;

public class ExampleUsageWithDI(LocalStorage localStorage, SessionStorage sessionStorage)
{
    private readonly LocalStorage localStorage = localStorage;
    private readonly SessionStorage sessionStorage = sessionStorage;

    public void UseStorage()
    {
        var value = localStorage.GetItem("storageKey");
        sessionStorage.SetItem("temporaryKey", value);
    }
}
