namespace JsBind.Net.Tests.InProjectBindings.Example2
{
    public class ExampleUsage
    {
        private readonly Global global;

        public ExampleUsage(Global global)
        {
            // Assuming Global has been registered in the dependency container
            this.global = global;
        }

        public void UseStorage()
        {
            var value = global.LocalStorage.GetItem("storageKey");
            global.SessionStorage.SetItem("temporaryKey", value);
        }
    }
}
