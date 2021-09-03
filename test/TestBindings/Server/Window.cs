using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JsBind.Net;

namespace TestBindings.Server
{
    [BindDeclaredProperties]
    public class Window : ObjectBindingBase
    {
        /// <summary>Parameterless constructor for when the instance is created by the JSON deserializer.</summary>
        public Window() { }

        /// <summary>This constructor for when the instance is created by the service provider.</summary>
        /// <param name="jsRuntimeAdapter">The JS runtime adapter.</param>
        public Window(IJsRuntimeAdapter jsRuntimeAdapter)
        {
            SetAccessPath("window");
            Initialize(jsRuntimeAdapter);
            // This constructor cannot initialize SOME property/field because it cannot use await in the constructor
            // This is not possible: Origin = await GetPropertyAsync<string>("origin")
            document = new Document(jsRuntimeAdapter);
        }

        // Property that is loaded when initialized, but ONLY from JSON deserializer, NOT the initializing constructor
        [JsonPropertyName("origin")]
        public string Origin { get; set; }

        // Property that is loaded everytime it is called (needs to be changed to async method)
        public ValueTask<Window> GetWindow() => GetPropertyAsync<Window>("window");

        // Property that is lazy loaded (needs to be changed to async method)
        private Document document;
        public async ValueTask<Document> GetDocument() => document ??= await GetPropertyAsync<Document>("document");

        // Invoke function on this object (needs to be changed to async method)
        public ValueTask<int> ParseInt(string value) => InvokeAsync<int>("parseInt", value);

        public ValueTask<TValue> GetVariableValue<TValue>(string variableName) => GetPropertyAsync<TValue>(variableName);
        public ValueTask SetVariableValue(string variableName, object variableValue) => SetPropertyAsync(variableName, variableValue);

        public ValueTask<TValue> ToType<TValue>() => ConvertToTypeAsync<TValue>();
    }
}
