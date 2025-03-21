using JsBind.Net.Tests.Infrastructure;
using TestBindings.Server;
using TestBindings.Server.BindingTestLibrary;

namespace JsBind.Net.Tests.Tests
{
    [TestClass(Description = "Advanced Asynchronous (Server)")]
    public class AdvancedTestAsynchronous
    {
        private readonly BindingTestLibrary bindingTestLibrary;
        private readonly Window window;

        public AdvancedTestAsynchronous(BindingTestLibrary bindingTestLibrary, Window window)
        {
            this.bindingTestLibrary = bindingTestLibrary;
            this.window = window;
        }

        [Fact(Description = "Plain object binding can be passed as argument")]
        public async Task PlainObjectBindingCanBePassedAsArgument()
        {
            // Arrange
            var obj = new TestBoundClass()
            {
                IsTestClass = true
            };

            // Act
            var result = await bindingTestLibrary.IsPlainObjectPassed(obj);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact(Description = "Object reference from function should be revived in JS")]
        public async Task ObjectReferenceFromFunctionShouldBeRevived()
        {
            // Arrange
            var obj = await bindingTestLibrary.GetTestObjectReviverInstanceFromFunction();

            // Act
            var isRevived = await bindingTestLibrary.IsObjectReferenceRevived(obj);

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Nested object reference from function should be revived in JS")]
        public async Task NestedObjectReferenceFromFunctionShouldBeRevived()
        {
            // Arrange
            var obj = await bindingTestLibrary.GetTestObjectReviverInstanceFromFunction();

            // Act
            var isRevived = await bindingTestLibrary.IsNestedObjectReferenceRevived(new
            {
                instance = obj
            });

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Object reference from property should be revived in JS")]
        public async Task ObjectReferenceFromPropertyShouldBeRevived()
        {
            // Arrange
            var obj = await bindingTestLibrary.GetTestObjectReviverInstanceFromProperty();

            // Act
            var isRevived = await bindingTestLibrary.IsObjectReferenceRevived(obj);

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Nested object reference from property should be revived in JS")]
        public async Task NestedObjectReferenceFromPropertyShouldBeRevived()
        {
            // Arrange
            var obj = await bindingTestLibrary.GetTestObjectReviverInstanceFromProperty();

            // Act
            var isRevived = await bindingTestLibrary.IsNestedObjectReferenceRevived(new
            {
                instance = obj
            });

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Delegate reference should be revived in JS")]
        public async Task DelegateReferenceShouldBeRevived()
        {
            // Arrange
            Action testDelegate = () => { };

            // Act
            var isRevived = await bindingTestLibrary.IsDelegateReferenceRevived(testDelegate);

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Nested delegate reference should be revived in JS")]
        public async Task NestedDelegateReferenceShouldBeRevived()
        {
            // Arrange
            static void testDelegate()
            {
                // For testing
            }

            // Act
            var isRevived = await bindingTestLibrary.IsNestedDelegateReferenceRevived(new
            {
                @delegate = (Action)testDelegate
            });

            // Assert
            isRevived.ShouldBeTrue();
        }

        [Fact(Description = "Delegate references should be equal in JS")]
        public async Task DelegateReferencesShouldBeEqual()
        {
            // Arrange
            Action testDelegate = () => { };

            // Act
            var isEqual = await bindingTestLibrary.AreDelegateReferencesEqual(testDelegate, testDelegate);

            // Assert
            isEqual.ShouldBeTrue();
        }

        [Fact(Description = "Delegate reference can be invoked from JS")]
        public async Task DelegateReferenceCanBeInvoked()
        {
            // Arrange
            var currentInvocationCount = 0;
            Action testDelegate = () => currentInvocationCount++;

            // Act
            await bindingTestLibrary.TestInvokeDelegate(testDelegate);

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
        public async Task JsInvokedDelegateReferenceWithPrimitiveReturnValue()
        {
            // Arrange
            var randomGuid = Guid.NewGuid();
            Func<Guid> testDelegate = () => randomGuid;

            // Act
            var result = await bindingTestLibrary.TestInvokeDelegate<Guid>(testDelegate);

            // Assert
            result.ShouldBe(randomGuid);
        }

        [Fact(Description = "JS invoked delegate reference with reference return value")]
        public async Task JsInvokedDelegateReferenceWithReferenceReturnValue()
        {
            // Arrange
            Func<Window> testDelegate = () => window;

            // Act
            var result = await bindingTestLibrary.TestInvokeDelegate<Window>(testDelegate);

            // Assert
            result.ShouldNotBeNull();
            (await result.InstanceEqualsAsync(window)).ShouldBeTrue();
        }

        [Fact(Description = "JS invoked delegate reference with delegate return value")]
        public async Task JsInvokedDelegateReferenceWithDelegateReturnValue()
        {
            // Arrange
            Action returnAction = () => { };
            Func<Action> testDelegate = () => returnAction;

            // Act
            var result = await bindingTestLibrary.TestInvokeDelegate<Action>(testDelegate);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeSameAs(returnAction);
        }

        [Fact(Description = "JS invoked delegate reference with nested delegate return value")]
        public async Task JsInvokedDelegateReferenceWithNestedDelegateReturnValue()
        {
            // Arrange
            Func<NestedDelegateClass> testDelegate = () => new()
            {
                NestedAction = async () => { await Task.Delay(1).ConfigureAwait(false); }
            };

            // Act
            var result = await bindingTestLibrary.TestInvokeDelegate<NestedDelegateClass>(testDelegate);

            // Assert
            result.ShouldNotBeNull();
            result.NestedAction.ShouldNotBeNull();
        }

        [Fact(Description = "Delegate return value from JS invoked delegate reference can be invoked")]
        public async Task DelegateReturnValueFromJsInvokedDelegateReferenceCanBeInvoked()
        {
            // Arrange
            var currentInvocationCount = 0;
            Func<Action> testDelegate = () => () => currentInvocationCount++;
            var delegateReturnValue = await bindingTestLibrary.TestInvokeDelegate<Action>(testDelegate);

            // Act
            delegateReturnValue();

            // Assert
            currentInvocationCount.ShouldBe(1);
        }

        [Fact(Description = "Delegate return value from JS invoked delegate reference can be invoked with primitive parameter")]
        public async Task DelegateReturnValueFromJsInvokedDelegateReferenceCanBeInvokedWithPrimitiveParameter()
        {
            // Arrange
            var currentInvocationCount = 0;
            Func<Action<int>> testDelegate = () => (val) => currentInvocationCount += val;
            var delegateReturnValue = await bindingTestLibrary.TestInvokeDelegate<Action<int>>(testDelegate);

            // Act
            delegateReturnValue(5);

            // Assert
            currentInvocationCount.ShouldBe(5);
        }

        [Fact(Description = "Delegate return value from JS invoked delegate reference can be invoked with reference parameter")]
        public async Task DelegateReturnValueFromJsInvokedDelegateReferenceCanBeInvokedWithReferenceParameter()
        {
            // Arrange
            var referenceParameterIsNotNull = false;
            Func<Action<Window>> testDelegate = () => (window) => referenceParameterIsNotNull = window is not null;
            var delegateReturnValue = await bindingTestLibrary.TestInvokeDelegate<Action<Window>>(testDelegate);

            // Act
            delegateReturnValue(window);

            // Assert
            referenceParameterIsNotNull.ShouldBeTrue();
        }

        [Fact(Description = "Invoke function with delegate return value")]
        public async Task InvokeFunctionWithDelegateReturnValue()
        {
            // Act
            var delegateReturnValue = await bindingTestLibrary.GetFunctionDelegate();

            // Assert
            delegateReturnValue.ShouldNotBeNull();
        }

        [Fact(Description = "Invoke function with nested delegate return value")]
        public async Task InvokeFunctionWithNestedDelegateReturnValue()
        {
            // Act
            var nestedDelegateReturnValue = await bindingTestLibrary.GetNestedActionDelegate();

            // Assert
            nestedDelegateReturnValue.ShouldNotBeNull();
            nestedDelegateReturnValue.NestedAction.ShouldNotBeNull();
        }

        [Fact(Description = "Returned delegate can be invoked")]
        public async Task ReturnedDelegateCanBeInvoked()
        {
            // Arrange
            var functionDelegate = await bindingTestLibrary.GetFunctionDelegate();

            // Act
            var result = await functionDelegate();

            // Assert
            result.ShouldBeTrue();
        }

        [Fact(Description = "Returned delegate can be invoked with primitive parameter and return value")]
        public async Task ReturnedDelegateCanBeInvokedWithPrimitiveParameterAndReturnValue()
        {
            // Arrange
            var functionDelegate = await bindingTestLibrary.GetPrimitiveFunctionDelegate();

            // Act
            var result = await functionDelegate(5);

            // Assert
            result.ShouldBe(5);
        }

        [Fact(Description = "Returned delegate can be invoked with reference parameter and return value")]
        public async Task ReturnedDelegateCanBeInvokedWithReferenceParameterAndReturnValue()
        {
            // Arrange
            var functionDelegate = await bindingTestLibrary.GetReferenceFunctionDelegate();

            // Act
            var result = await functionDelegate(window);

            // Assert
            (await result.InstanceEqualsAsync(window)).ShouldBeTrue();
        }

        [Fact(Description = "Complex object can be passed round trip")]
        public async Task ComplexObjectCanBePassedRoundTrip()
        {
            // Arrange
            var functionDelegate = await bindingTestLibrary.GetMirrorFunctionDelegate<ComplexTestBoundClass>();
            var obj = new ComplexTestBoundClass()
            {
                NestedItem = new() { Property = "NestedItemProperty" },
                NestedItemsArray = [new() { Property = "NestedItemArrayProperty" }],
                NestedItemsEnumerable = [new() { Property = "NestedItemEnumerableProperty" }],
                NestedItemsList = [new() { Property = "NestedItemListProperty" }]
            };

            // Act
            var result = await functionDelegate(obj);

            // Assert
            result.NestedItem.ShouldBeEquivalentTo(obj.NestedItem);
            result.NestedItemsArray.ShouldBe(obj.NestedItemsArray);
            result.NestedItemsEnumerable.ShouldBe(obj.NestedItemsEnumerable);
            result.NestedItemsList.ShouldBe(obj.NestedItemsList);
        }
    }
}
