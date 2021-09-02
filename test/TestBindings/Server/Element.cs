using System.Threading.Tasks;
using JsBind.Net;

namespace TestBindings.Server
{
    [BindDeclaredProperties]
    public class Element : ObjectBindingBase
    {
        // Property that is loaded when initialized from JSON deserializer
        public string Id { get; set; }

        // Property that is loaded everytime it is called (needs to be changed to async method)
        public ValueTask<string> GetTagName() => GetPropertyAsync<string>("tagName");

        // Invoke function on this object (needs to be changed to async method)
        public ValueTask<string> GetAttribute(string attributeName) => InvokeAsync<string>("getAttribute", attributeName);
    }
}
