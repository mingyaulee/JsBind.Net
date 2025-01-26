using JsBind.Net.Tests.Infrastructure;
using TestBindings.Server;
using TestBindings.Server.BindingTestLibrary;

namespace JsBind.Net.Tests.Tests
{
    [TestClass(Description = "Object Manager Synchronous (WebAssembly)")]
    public class ObjectManagerTestAsynchronous
    {
        private readonly Document document;
        private readonly BindingTestLibrary bindingTestLibrary;
        private readonly IJsRuntimeAdapter jsRuntime;
        private readonly Func<ValueTask<int>> getObjectReferencesCount;
        private readonly Func<ValueTask<int>> getDelegateReferencesCount;

        public ObjectManagerTestAsynchronous(Document document, BindingTestLibrary bindingTestLibrary, IJsRuntimeAdapter jsRuntime)
        {
            this.document = document;
            this.bindingTestLibrary = bindingTestLibrary;
            this.jsRuntime = jsRuntime;
            getObjectReferencesCount = () => Any.From("JsBindNet", jsRuntime).InvokeFunctionAsync<int>("getObjectReferencesCount");
            getDelegateReferencesCount = () => Any.From("JsBindNet", jsRuntime).InvokeFunctionAsync<int>("getDelegateReferencesCount");
        }

        [Fact(Description = "Dispose object reference")]
        public async Task DisposeObjectReference()
        {
            // Arrange
            var objectReference = await document.GetElementById("app");
            var objectReferencesCount = await getObjectReferencesCount();

            // Act
            await JsObjectManager.DisposeObjectReferenceAsync(objectReference);
            var currentObjectReferencesCount = await getObjectReferencesCount();

            // Assert
            currentObjectReferencesCount.ShouldBe(objectReferencesCount - 1);
        }

        [Fact(Description = "Dispose array like object reference")]
        public async Task DisposeArrayLikeObjectReference()
        {
            // Arrange
            var arrayObjectReference = await document.QuerySelectorAll("#app");
            var objectReferencesCount = await getObjectReferencesCount();

            // Act
            await JsObjectManager.DisposeObjectReferenceAsync(arrayObjectReference);
            var currentObjectReferencesCount = await getObjectReferencesCount();

            // Assert
            currentObjectReferencesCount.ShouldBe(objectReferencesCount - 1);
        }

        [Fact(Description = "Dispose root object reference")]
        public async Task DisposeRootObjectReference()
        {
            // Arrange
            var objectReference = await document.GetElementById("app");
            var objectReferencesCount = await getObjectReferencesCount();

            // Act
            await JsObjectManager.DisposeRootObjectReferenceAsync(objectReference.Attributes);
            var currentObjectReferencesCount = await getObjectReferencesCount();

            // Assert
            currentObjectReferencesCount.ShouldBe(objectReferencesCount - 1);
        }

        [Fact(Description = "Dispose delegate reference")]
        public async Task DisposeDelegateReference()
        {
            // Arrange
            static void delegateReference()
            {
                // For testing
            }
            await bindingTestLibrary.TestInvokeDelegate((Action)delegateReference);
            var delegateReferencesCount = await getDelegateReferencesCount();

            // Act
            await JsObjectManager.DisposeDelegateReferenceAsync((Action)delegateReference, jsRuntime);
            var currentDelegateReferencesCount = await getDelegateReferencesCount();

            // Assert
            currentDelegateReferencesCount.ShouldBe(delegateReferencesCount - 1);
        }

        [Fact(Description = "Dispose session")]
        public async Task DisposeSession()
        {
            // Arrange
            await document.GetElementById("app");
            static void delegateReference()
            {
                // For testing
            }
            await bindingTestLibrary.TestInvokeDelegate((Action)delegateReference);

            // Act
            await JsObjectManager.DisposeSessionAsync(jsRuntime, disposeJsReferences: true);
            var currentObjectReferencesCount = await getObjectReferencesCount();
            var currentDelegateReferencesCount = await getDelegateReferencesCount();

            // Assert
            currentObjectReferencesCount.ShouldBe(0);
            currentDelegateReferencesCount.ShouldBe(0);
        }
    }
}
