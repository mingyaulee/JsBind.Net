using System.Text.Json.Serialization;
using JsBind.Net;

namespace TestBindings.WebAssembly
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
            // This constructor can initialize ANY property/field
            Origin = GetProperty<string>("origin");
            document = new Document(jsRuntimeAdapter);
        }

        // Property that is loaded when initialized, either from JSON deserializer or from the initializing constructor
        [JsonPropertyName("origin")]
        public string Origin { get; set; }

        // Property that is loaded everytime it is called
        public Window window => GetProperty<Window>(nameof(window));

        // Property that is lazy loaded
        private Document document;
        public Document Document => document ??= GetProperty<Document>("document");

        // Invoke function on this object
        public int ParseInt(string value) => Invoke<int>("parseInt", value);

        public TValue GetVariableValue<TValue>(string variableName) => GetProperty<TValue>(variableName);
        public void SetVariableValue(string variableName, object variableValue) => SetProperty(variableName, variableValue);
    }
}
