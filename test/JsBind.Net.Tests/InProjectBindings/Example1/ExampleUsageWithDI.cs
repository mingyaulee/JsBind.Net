namespace JsBind.Net.Tests.InProjectBindings.Example1
{
    public class ExampleUsageWithDI
    {
        private readonly LocalStorage localStorage;
        private readonly SessionStorage sessionStorage;

        public ExampleUsageWithDI(LocalStorage localStorage, SessionStorage sessionStorage)
        {
            // Assuming LocalStorage and SessionStorage have been registered in the dependency container
            this.localStorage = localStorage;
            this.sessionStorage = sessionStorage;
        }

        public void UseStorage()
        {
            var value = localStorage.GetItem("storageKey");
            sessionStorage.SetItem("temporaryKey", value);
        }
    }
}
