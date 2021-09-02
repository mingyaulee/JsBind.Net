using System.Collections.Generic;
using System.Threading.Tasks;
using JsBind.Net;

namespace TestBindings.Server
{
    [BindDeclaredProperties]
    public class Document : ObjectBindingBase
    {
        /// <summary>Parameterless constructor for when the instance is created by the JSON deserializer.</summary>
        public Document() { }

        /// <summary>This constructor for when the instance is created by the service provider.</summary>
        /// <param name="jsRuntimeAdapter">The JS runtime adapter.</param>
        public Document(IJsRuntimeAdapter jsRuntimeAdapter)
        {
            SetAccessPath("document");
            Initialize(jsRuntimeAdapter);
        }

        // Invoke function on this object with reference return type (needs to be changed to async method)
        public ValueTask<Element> GetElementById(string id) => InvokeAsync<Element>("getElementById", id);

        // Invoke function on this object with array like return type (needs to be changed to async method)
        public ValueTask<IEnumerable<Element>> QuerySelectorAll(string selector) => InvokeAsync<IEnumerable<Element>>("querySelectorAll", selector);
    }
}
