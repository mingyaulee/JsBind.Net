namespace JsBind.Net.Tests.InProjectBindings.Example2
{
    public class ExampleUsage(Global global)
    {
        private readonly Global global = global;

        public void UseStorage()
        {
            var value = global.LocalStorage.GetItem("storageKey");
            global.SessionStorage.SetItem("temporaryKey", value);
        }
    }
}
