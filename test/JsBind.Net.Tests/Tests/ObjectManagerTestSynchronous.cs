using System;
using FluentAssertions;
using JsBind.Net.Tests.Infrastructure;
using TestBindings.WebAssembly;
using TestBindings.WebAssembly.BindingTestLibrary;

namespace JsBind.Net.Tests.Tests
{
    [TestClass(Description = "Object Manager Synchronous (WebAssembly)")]
    public class ObjectManagerTestSynchronous
    {
        private readonly Document document;
        private readonly BindingTestLibrary bindingTestLibrary;
        private readonly IJsRuntimeAdapter jsRuntime;
        private readonly Func<int> getObjectReferencesCount;
        private readonly Func<int> getDelegateReferencesCount;

        public ObjectManagerTestSynchronous(Document document, BindingTestLibrary bindingTestLibrary, IJsRuntimeAdapter jsRuntime)
        {
            this.document = document;
            this.bindingTestLibrary = bindingTestLibrary;
            this.jsRuntime = jsRuntime;
            getObjectReferencesCount = () => Any.From("JsBindNet", jsRuntime).InvokeFunction<int>("getObjectReferencesCount");
            getDelegateReferencesCount = () => Any.From("JsBindNet", jsRuntime).InvokeFunction<int>("getDelegateReferencesCount");
        }

        [Fact(Description = "Dispose object reference")]
        public void DisposeObjectReference()
        {
            // Arrange
            var objectReference = document.GetElementById("app");
            var objectReferencesCount = getObjectReferencesCount();

            // Act
            JsObjectManager.DisposeObjectReference(objectReference);
            var currentObjectReferencesCount = getObjectReferencesCount();

            // Assert
            currentObjectReferencesCount.Should().Be(objectReferencesCount - 1);
        }

        [Fact(Description = "Dispose array like object reference")]
        public void DisposeArrayLikeObjectReference()
        {
            // Arrange
            var arrayObjectReference = document.QuerySelectorAll("#app");
            var objectReferencesCount = getObjectReferencesCount();

            // Act
            JsObjectManager.DisposeObjectReference(arrayObjectReference);
            var currentObjectReferencesCount = getObjectReferencesCount();

            // Assert
            currentObjectReferencesCount.Should().Be(objectReferencesCount - 1);
        }

        [Fact(Description = "Dispose root object reference")]
        public void DisposeRootObjectReference()
        {
            // Arrange
            var objectReference = document.GetElementById("app");
            var objectReferencesCount = getObjectReferencesCount();

            // Act
            JsObjectManager.DisposeRootObjectReference(objectReference.Attributes);
            var currentObjectReferencesCount = getObjectReferencesCount();

            // Assert
            currentObjectReferencesCount.Should().Be(objectReferencesCount - 1);
        }

        [Fact(Description = "Dispose delegate reference")]
        public void DisposeDelegateReference()
        {
            // Arrange
            static void delegateReference()
            {
                // For testing
            }
            bindingTestLibrary.TestInvokeDelegate((Action)delegateReference);
            var delegateReferencesCount = getDelegateReferencesCount();

            // Act
            JsObjectManager.DisposeDelegateReference((Action)delegateReference, jsRuntime);
            var currentDelegateReferencesCount = getDelegateReferencesCount();

            // Assert
            currentDelegateReferencesCount.Should().Be(delegateReferencesCount - 1);
        }

        [Fact(Description = "Dispose session")]
        public void DisposeSession()
        {
            // Arrange
            document.GetElementById("app");
            static void delegateReference()
            {
                // For testing
            }
            bindingTestLibrary.TestInvokeDelegate((Action)delegateReference);

            // Act
            JsObjectManager.DisposeSession(jsRuntime, disposeJsReferences: true);
            var currentObjectReferencesCount = getObjectReferencesCount();
            var currentDelegateReferencesCount = getDelegateReferencesCount();

            // Assert
            currentObjectReferencesCount.Should().Be(0);
            currentDelegateReferencesCount.Should().Be(0);
        }
    }
}
