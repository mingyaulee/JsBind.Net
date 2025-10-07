using JsBind.Net.Tests.Infrastructure;
using TestBindings.WebAssembly;
using TestBindings.WebAssembly.BindingTestLibrary;

namespace JsBind.Net.Tests.Tests
{
    [TestClass(Description = "Advanced Synchronous (WebAssembly)")]
    public class AdvancedTestSynchronous(BindingTestLibrary bindingTestLibrary, Window window)
    {
        private readonly BindingTestLibrary bindingTestLibrary = bindingTestLibrary;
        private readonly Window window = window;

        [Fact(Description = "Plain object binding can be passed as argument")]
        public void PlainObjectBindingCanBePassedAsArgument()
        {
            // Arrange
            var obj = new TestBoundClass()
            {
                IsTestClass = true
            };

            // Act
            var result = bindingTestLibrary.IsPlainObjectPassed(obj);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact(Description = "Object reference from function should be revived in JS")]
        public void ObjectReferenceFromFunctionShouldBeRevived()
        {
            // Arrange
            var obj = bindingTestLibrary.GetTestObjectReviverInstanceFromFunction();

            // Act
            var isRevived = bindingTestLibrary.IsObjectReferenceRevived(obj);

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Nested object reference from function should be revived in JS")]
        public void NestedObjectReferenceFromFunctionShouldBeRevived()
        {
            // Arrange
            var obj = bindingTestLibrary.GetTestObjectReviverInstanceFromFunction();

            // Act
            var isRevived = bindingTestLibrary.IsNestedObjectReferenceRevived(new
            {
                instance = obj
            });

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Object reference from property should be revived in JS")]
        public void ObjectReferenceFromPropertyShouldBeRevived()
        {
            // Arrange
            var obj = bindingTestLibrary.TestObjectReviverInstanceFromProperty;

            // Act
            var isRevived = bindingTestLibrary.IsObjectReferenceRevived(obj);

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Nested object reference from property should be revived in JS")]
        public void NestedObjectReferenceFromPropertyShouldBeRevived()
        {
            // Arrange
            var obj = bindingTestLibrary.TestObjectReviverInstanceFromProperty;

            // Act
            var isRevived = bindingTestLibrary.IsNestedObjectReferenceRevived(new
            {
                instance = obj
            });

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Delegate reference should be revived in JS")]
        public void DelegateReferenceShouldBeRevived()
        {
            // Arrange
            Action testDelegate = () => { };

            // Act
            var isRevived = bindingTestLibrary.IsDelegateReferenceRevived(testDelegate);

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Nested delegate reference should be revived in JS")]
        public void NestedDelegateReferenceShouldBeRevived()
        {
            // Arrange
            static void testDelegate()
            {
                // For testing
            }

            // Act
            var isRevived = bindingTestLibrary.IsNestedDelegateReferenceRevived(new
            {
                @delegate = (Action)testDelegate
            });

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Delegate references should be equal in JS")]
        public void DelegateReferencesShouldBeEqual()
        {
            // Arrange
            Action testDelegate = () => { };

            // Act
            var isEqual = bindingTestLibrary.AreDelegateReferencesEqual(testDelegate, testDelegate);

            // Assert
            isEqual.ShouldBeTrue();
        }

        [Fact(Description = "Delegate reference can be invoked from JS")]
        public void DelegateReferenceCanBeInvoked()
        {
            // Arrange
            var currentInvocationCount = 0;
            Action testDelegate = () => currentInvocationCount++;

            // Act
            bindingTestLibrary.TestInvokeDelegate(testDelegate);

            // Assert
            currentInvocationCount.ShouldBe(1);
        }

        [Fact(Description = "Async delegate reference can be invoked from JS")]
        public async Task AsyncDelegateReferenceCanBeInvoked()
        {
            // Arrange
            var currentInvocationCount = 0;
            Func<ValueTask> testDelegateAsync = () =>
            {
                currentInvocationCount++;
                return ValueTask.CompletedTask;
            };

            // Act - we are not able to use await directly on the invocation as it will cause a deadlock in single threaded environment
            await bindingTestLibrary.TestInvokeDelegateAsync(testDelegateAsync);

            // Assert
            currentInvocationCount.ShouldBe(1);
        }

        [Fact(Description = "JS invoked delegate reference with primitive return value")]
        public void JsInvokedDelegateReferenceWithPrimitiveReturnValue()
        {
            // Arrange
            var randomGuid = Guid.NewGuid();
            Func<Guid> testDelegate = () => randomGuid;

            // Act
            var result = bindingTestLibrary.TestInvokeDelegate<Guid>(testDelegate);

            // Assert
            result.ShouldBe(randomGuid);
        }

        [Fact(Description = "JS invoked delegate reference with reference return value")]
        public void JsInvokedDelegateReferenceWithReferenceReturnValue()
        {
            // Arrange
            Func<Window> testDelegate = () => window;

            // Act
            var result = bindingTestLibrary.TestInvokeDelegate<Window>(testDelegate);

            // Assert
            result.ShouldNotBeNull();
            result.InstanceEquals(window).ShouldBeTrue();
        }

        [Fact(Description = "JS invoked delegate reference with delegate return value")]
        public void JsInvokedDelegateReferenceWithDelegateReturnValue()
        {
            // Arrange
            Action returnAction = () => { };
            Func<Action> testDelegate = () => returnAction;

            // Act
            var result = bindingTestLibrary.TestInvokeDelegate<Action>(testDelegate);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeSameAs(returnAction);
        }

        [Fact(Description = "JS invoked delegate reference with nested delegate return value")]
        public void JsInvokedDelegateReferenceWithNestedDelegateReturnValue()
        {
            // Arrange
            Func<NestedDelegateClass> testDelegate = () => new()
            {
                NestedAction = () => { }
            };

            // Act
            var result = bindingTestLibrary.TestInvokeDelegate<NestedDelegateClass>(testDelegate);

            // Assert
            result.ShouldNotBeNull();
            result.NestedAction.ShouldNotBeNull();
        }

        [Fact(Description = "Delegate return value from JS invoked delegate reference can be invoked")]
        public void DelegateReturnValueFromJsInvokedDelegateReferenceCanBeInvoked()
        {
            // Arrange
            var currentInvocationCount = 0;
            Func<Action> testDelegate = () => () => currentInvocationCount++;
            var delegateReturnValue = bindingTestLibrary.TestInvokeDelegate<Action>(testDelegate);

            // Act
            delegateReturnValue();

            // Assert
            currentInvocationCount.ShouldBe(1);
        }

        [Fact(Description = "Delegate return value from JS invoked delegate reference can be invoked with primitive parameter")]
        public void DelegateReturnValueFromJsInvokedDelegateReferenceCanBeInvokedWithPrimitiveParameter()
        {
            // Arrange
            var currentInvocationCount = 0;
            Func<Action<int>> testDelegate = () => (val) => currentInvocationCount += val;
            var delegateReturnValue = bindingTestLibrary.TestInvokeDelegate<Action<int>>(testDelegate);

            // Act
            delegateReturnValue(5);

            // Assert
            currentInvocationCount.ShouldBe(5);
        }

        [Fact(Description = "Delegate return value from JS invoked delegate reference can be invoked with reference parameter")]
        public void DelegateReturnValueFromJsInvokedDelegateReferenceCanBeInvokedWithReferenceParameter()
        {
            // Arrange
            var referenceParameterIsNotNull = false;
            Func<Action<Window>> testDelegate = () => (window) => referenceParameterIsNotNull = window is not null;
            var delegateReturnValue = bindingTestLibrary.TestInvokeDelegate<Action<Window>>(testDelegate);

            // Act
            delegateReturnValue(window);

            // Assert
            referenceParameterIsNotNull.ShouldBeTrue();
        }

        [Fact(Description = "Invoke function with delegate return value")]
        public void InvokeFunctionWithDelegateReturnValue()
        {
            // Act
            var delegateReturnValue = bindingTestLibrary.GetFunctionDelegate();

            // Assert
            delegateReturnValue.ShouldNotBeNull();
        }

        [Fact(Description = "Invoke function with nested delegate return value")]
        public void InvokeFunctionWithNestedDelegateReturnValue()
        {
            // Act
            var nestedDelegateReturnValue = bindingTestLibrary.GetNestedActionDelegate();

            // Assert
            nestedDelegateReturnValue.ShouldNotBeNull();
            nestedDelegateReturnValue.NestedAction.ShouldNotBeNull();
        }

        [Fact(Description = "Returned delegate can be invoked")]
        public void ReturnedDelegateCanBeInvoked()
        {
            // Arrange
            var functionDelegate = bindingTestLibrary.GetFunctionDelegate();

            // Act
            var result = functionDelegate();

            // Assert
            result.ShouldBeTrue();
        }

        [Fact(Description = "Returned delegate can be invoked with primitive parameter and return value")]
        public void ReturnedDelegateCanBeInvokedWithPrimitiveParameterAndReturnValue()
        {
            // Arrange
            var functionDelegate = bindingTestLibrary.GetPrimitiveFunctionDelegate();

            // Act
            var result = functionDelegate(5);

            // Assert
            result.ShouldBe(5);
        }

        [Fact(Description = "Returned delegate can be invoked with reference parameter and return value")]
        public void ReturnedDelegateCanBeInvokedWithReferenceParameterAndReturnValue()
        {
            // Arrange
            var functionDelegate = bindingTestLibrary.GetReferenceFunctionDelegate();

            // Act
            var result = functionDelegate(window);

            // Assert
            result.InstanceEquals(window).ShouldBeTrue();
        }

        [Fact(Description = "Complex object can be passed round trip")]
        public void ComplexObjectCanBePassedRoundTrip()
        {
            // Arrange
            var functionDelegate = bindingTestLibrary.GetMirrorFunctionDelegate<ComplexTestBoundClass>();
            var obj = new ComplexTestBoundClass()
            {
                NestedItem = new() { Property = "NestedItemProperty" },
                NestedItemsArray = [new() { Property = "NestedItemArrayProperty" }],
                NestedItemsEnumerable = [new() { Property = "NestedItemEnumerableProperty" }],
                NestedItemsList = [new() { Property = "NestedItemListProperty" }]
            };

            // Act
            var result = functionDelegate(obj);

            // Assert
            result.NestedItem.ShouldBeEquivalentTo(obj.NestedItem);
            result.NestedItemsArray.ShouldBe(obj.NestedItemsArray);
            result.NestedItemsEnumerable.ShouldBe(obj.NestedItemsEnumerable);
            result.NestedItemsList.ShouldBe(obj.NestedItemsList);
        }
    }
}
