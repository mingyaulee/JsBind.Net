using System.Collections.Generic;
using JsBind.Net;

namespace TestBindings.WebAssembly
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

        // Invoke function on this object with reference return type
        public Element GetElementById(string id) => Invoke<Element>("getElementById", id);

        // Invoke function on this object with array like return type
        public IEnumerable<Element> QuerySelectorAll(string selector) => Invoke<IEnumerable<Element>>("querySelectorAll", selector);
    }
}
