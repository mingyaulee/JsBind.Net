using JsBind.Net;

namespace TestBindings.WebAssembly
{
    [BindDeclaredProperties]
    public class Element : ObjectBindingBase<Element>
    {
        // Property that is loaded when initialized from JSON deserializer
        public string Id { get; set; }

        // Property that is loaded everytime it is called
        public string TagName => GetProperty<string>("tagName");

        // Invoke function on this object
        public string GetAttribute(string attributeName) => Invoke<string>("getAttribute", attributeName);
    }
}
